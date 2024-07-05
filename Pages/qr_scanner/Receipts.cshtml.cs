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

        public async Task OnGet()
        {
            receipts = await _db.Receipt.ToListAsync();
        }

    }
}