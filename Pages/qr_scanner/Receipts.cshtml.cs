using OfficeOpenXml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using qr_scanner_app_staj.Model;

namespace qr_scanner_app_staj.Pages.qr_scanner
{
    public class ReceiptsModel(ApplicationDbContext db) : PageModel
    {
        private readonly ApplicationDbContext _db = db;

        public IEnumerable<Receipt> Receipts { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUser");

            if (currentUserId.HasValue)
            {
                Receipts = await _db.Receipt
                                    .Where(r => r.userId == currentUserId.Value)
                                    .ToListAsync();

                return Page();
            }

            return RedirectToPage("Index");
        }

        public IActionResult OnPostLogOut()
        {
            HttpContext.Session.Remove("CurrentUser");
            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostExcelAsync()
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUser");

            if (!currentUserId.HasValue)
            {
                return RedirectToPage("Index");
            }

            Receipts = await _db.Receipt
                                .Where(r => r.userId == currentUserId.Value)
                                .ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Receipts");
                var header = new[] { "Receipt ID", "Date", "Total", "Total Tax" };
                for (int j = 0; j < header.Length; j++)
                {
                    worksheet.Cells[1, j + 1].Value = header[j];
                }

                for (int i = 0; i < Receipts.Count(); i++)
                {
                    var receipt = Receipts.ElementAt(i);
                    worksheet.Cells[i + 2, 1].Value = receipt.receiptId;
                    worksheet.Cells[i + 2, 2].Value = receipt.date;
                    worksheet.Cells[i + 2, 3].Value = receipt.total;
                    worksheet.Cells[i + 2, 4].Value = receipt.totalTax;
                }

                var user = await _db.User
                                    .Where(u => u.userId == currentUserId.Value)
                                    .Select(u => new { u.username })
                                    .FirstOrDefaultAsync();

                if (user != null)
                {
                    string sanitizedUsername = string.Join("_", user.username.Split(Path.GetInvalidFileNameChars()));
                    string fileName = $"ExcelOutputs/output_{sanitizedUsername}.xlsx";

                    var fileInfo = new FileInfo(fileName);
                    package.SaveAs(fileInfo);
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(fileName, FileMode.Open))
                    {
                        stream.CopyTo(memory);
                    }
                    memory.Position = 0;

                    return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"excel_export_{sanitizedUsername}.xlsx");
                }
            }

            return Page();
        }
    }
}
