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

        public IEnumerable<Receipt> receipts { get; set; }

        public async Task<IActionResult> OnGet()
        {
            if (HttpContext.Session.GetInt32("CurrentUser").HasValue)
            {
                receipts = await _db.Receipt.Where(r => r.userId == HttpContext.Session.GetInt32("CurrentUser")).ToListAsync();
                return Page();
            }
            else
            {
                return RedirectToPage("Index");
            }
        }
        public IActionResult OnPostLogOut()
        {
            HttpContext.Session.Remove("CurrentUser");
            return RedirectToPage("Index");
        }
        public async Task OnPostExcelAsync()
        {
            receipts = await _db.Receipt.Where(r => r.userId == HttpContext.Session.GetInt32("CurrentUser")).ToListAsync();
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet");
                int i = 0;
                foreach (var receipt in receipts)
                {
                    worksheet.Cells[i + 1, 1].Value = receipt.receiptId;
                    worksheet.Cells[i + 1, 2].Value = receipt.date;
                    worksheet.Cells[i + 1, 3].Value = receipt.total;
                    worksheet.Cells[i + 1, 4].Value = receipt.totalTax;
                    i++;
                }
                var user = await _db.User
                        .Where(u => u.userId == HttpContext.Session.GetInt32("CurrentUser"))
                        .Select(u => new { u.username })
                        .FirstOrDefaultAsync();
                string sanitizedUsername = string.Join("_", user.username.Split(Path.GetInvalidFileNameChars()));
                string fileName = $"ExcelOutputs/output_{sanitizedUsername}.xlsx";
                FileInfo fileInfo = new FileInfo(fileName);
                package.SaveAs(fileInfo);
            }
        }
    }
}