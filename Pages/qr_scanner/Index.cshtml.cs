using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using qr_scanner_app_staj.Model;
namespace qr_scanner_app_staj.Pages.qr_scanner
{
    public class IndexModel(ApplicationDbContext db) : PageModel
    {
        private readonly ApplicationDbContext _db = db;
        [BindProperty]
        public User User { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var user = await _db.User.SingleOrDefaultAsync(u => u.username == User.username && u.password == User.password);
                if (user == null)
                {
                    return Page();
                }
                return RedirectToPage("Receipts");
            }
            else
            {
                return Page();
            }
        }
    }
}