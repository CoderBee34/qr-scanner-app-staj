using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using qr_scanner_app_staj.Model;

namespace qr_scanner_app_staj.Pages.qr_scanner
{
    public class ScannerModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public string QrCode { get; set; }

        public ScannerModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("CurrentUser").HasValue)
            {
                return Page();
            }
            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string pictureURL = $"https://monitoring.e-kassa.gov.az/pks-monitoring/2.0.0/documents/{GetDocParameter(QrCode)}";

            Console.WriteLine($"QR Code: {QrCode}");
            Console.WriteLine($"Picture URL: {pictureURL}");

            string filePath = Path.Combine(Environment.CurrentDirectory, "downloaded_image.jpg");

            await DownloadImageAsync(pictureURL, filePath);

            string extractedText = ExtractTextFromImage(filePath);
            Console.WriteLine("Extracted Text:");
            Console.WriteLine(extractedText);

            string receiptNo = ExtractReceiptDetail(extractedText, @"Salle receipt Ne\s*(\d+)");
            string date = ExtractReceiptDetail(extractedText, @"Date:\s*(\d{2}\.\d{2}\.\d{4})");
            string total = ExtractReceiptDetail(extractedText, @"Total\s*([\d.]+)");
            string totalTax = ExtractReceiptDetail(extractedText, @"Total tax\s*=\s*([\d.]+)");

            int userId = HttpContext.Session.GetInt32("CurrentUser").Value;

            var receipt = await _db.Receipt
                .SingleOrDefaultAsync(r => r.receiptId == int.Parse(receiptNo) && r.userId == userId);

            if (receipt == null)
            {
                TempData["ErrorMessage"] = "This receipt already exists in the database.";
                var newReceipt = new Receipt(int.Parse(receiptNo), date, double.Parse(total), double.Parse(totalTax), userId);
                await _db.AddAsync(newReceipt);
                await _db.SaveChangesAsync();
            }

            return RedirectToPage("Receipts");
        }

        private async Task DownloadImageAsync(string url, string filePath)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
                Console.WriteLine($"Image saved to {filePath}");
            }
        }

        private static string ExtractTextFromImage(string imagePath)
        {
            string pythonScriptPath = "Pages/python_script/ImageTextExtractor.py";

            var start = new ProcessStartInfo
            {
                FileName = "/usr/local/bin/python3",
                Arguments = $"\"{pythonScriptPath}\" \"{imagePath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            using (var process = Process.Start(start))
            using (var reader = process.StandardOutput)
            {
                return reader.ReadToEnd();
            }
        }

        private static string ExtractReceiptDetail(string text, string pattern)
        {
            var match = Regex.Match(text, pattern);
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        private static string GetDocParameter(string url)
        {
            Uri uri = new Uri(url);
            string fragment = uri.Fragment;
            string[] parameters = fragment.Split('&');

            foreach (string param in parameters)
            {
                if (param.Contains("doc="))
                {
                    return param.Substring(param.IndexOf("doc=") + 4);
                }
            }
            return null;
        }
    }
}