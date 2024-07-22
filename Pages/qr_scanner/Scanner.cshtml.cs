using System.Diagnostics;
using System.Text.RegularExpressions;
using IronOcr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using qr_scanner_app_staj.Model;
namespace qr_scanner_app_staj.Pages.qr_scanner
{
    public class ScannerModel(ApplicationDbContext db) : PageModel
    {
        private readonly ApplicationDbContext _db = db;
        [BindProperty]
        public string QrCode { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("CurrentUser").HasValue)
            {
                return Page();
            }
            else
            {
                return RedirectToPage("Index");
            }
        }
        public async Task<IActionResult> OnPost()
        {
            string pictureURL = "https://monitoring.e-kassa.gov.az/pks-monitoring/2.0.0/documents/";
            pictureURL = pictureURL + GetDocParameter(QrCode);
            Console.WriteLine(QrCode);
            Console.WriteLine(pictureURL);
            string filePath = "";

            IWebDriver driver = new ChromeDriver();
            try
            {
                driver.Navigate().GoToUrl(pictureURL);
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                filePath = Path.Combine(Environment.CurrentDirectory, "screenshot.png");
                screenshot.SaveAsFile(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }
            finally
            {
                driver.Quit();
            }

            string extractedText = ExtractTextFromImage(filePath);
            Console.WriteLine("Extracted Text:");
            Console.WriteLine(extractedText);

            string receiptNoPattern = @"Sale receipt Ne\s*(\d+)";
            string receiptNo = Regex.Match(extractedText, receiptNoPattern).Groups[1].Value;

            string datePattern = @"Date\s*(\d{2}\.\d{2}\.\d{4})";
            string date = Regex.Match(extractedText, datePattern).Groups[1].Value;

            string totalPattern = @"Total\s*(\d+\.\d+)";
            string total = Regex.Match(extractedText, totalPattern).Groups[1].Value;

            string totalTaxPattern = @"\*Total tax\s*=\s*(\d+\.\d+)";
            string totalTax = Regex.Match(extractedText, totalTaxPattern).Groups[1].Value;

            int userId = HttpContext.Session.GetInt32("CurrentUser").Value;
            var receipt = await _db.Receipt.SingleOrDefaultAsync(u => u.receiptId == int.Parse(receiptNo));
            if (receipt == null)
            {
                TempData["ErrorMessage"] = "This receipt already exists on database.";
                await _db.AddAsync(new Receipt(int.Parse(receiptNo), date, double.Parse(total), double.Parse(totalTax), userId));
                await _db.SaveChangesAsync();
            }

            return RedirectToPage("Receipts");
        }

        private static string ExtractTextFromImage(string imagePath)
        {

            string pythonScriptPath = "Pages/python_script/ImageTextExtractor.py";

            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = "/usr/local/bin/python3",
                Arguments = $"{pythonScriptPath} \"{imagePath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            string result;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    result = reader.ReadToEnd();
                }
            }
            return result;
        }
        static string GetDocParameter(string url)
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