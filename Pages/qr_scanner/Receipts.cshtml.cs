using System.Net.Http.Headers;
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
    }
}