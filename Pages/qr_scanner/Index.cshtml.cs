using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using qr_scanner_app_staj.Model;
namespace qr_scanner_app_staj.Pages.qr_scanner
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public User User { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("CurrentUser").HasValue)
            {
                return RedirectToPage("Receipts");
            }
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (User == null)
            {
                TempData["ErrorMessage"] = "User details are not provided.";
                return Page();
            }

            if (ModelState.IsValid)
            {
                var user = await _db.User.SingleOrDefaultAsync(u => u.username == User.username);
                if (user == null || user.PasswordHash == null || !BCrypt.Net.BCrypt.Verify(User.PasswordHash, user.PasswordHash))
                {
                    TempData["ErrorMessage"] = "Wrong username or password.";
                    return Page();
                }
                HttpContext.Session.SetInt32("CurrentUser", user.userId);
                return RedirectToPage("Receipts");
            }
            else
            {
                return Page();
            }
        }
    }
}