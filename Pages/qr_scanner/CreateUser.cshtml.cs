using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using qr_scanner_app_staj.Model;

namespace qr_scanner_app_staj.Pages.qr_scanner
{
    public class CreateUser(ApplicationDbContext db) : PageModel
    {
        private readonly ApplicationDbContext _db = db;
        [BindProperty]
        public FormModel FormModel { get; set; }
        public int AvailableUserID { get; set; }
        public void OnGet()
        {
            AvailableUserID = (_db.User.Any() ? _db.User.Max(u => u.userId) : 0) + 1;
        }

        public IActionResult OnPost()
        {
            string username = FormModel.Username;
            string password = FormModel.Password;
            string rePassword = FormModel.RePassword;

            if (!password.Equals(rePassword))
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return Page();
            }

            if (_db.User.Any(u => u.username == username && u.PasswordHash == BCrypt.Net.BCrypt.HashPassword(password)))
            {
                ModelState.AddModelError(string.Empty, "User already exists.");
                return Page();
            }

            var newUser = new User
            {
                userId = AvailableUserID,
                username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            _db.User.Add(newUser);
            _db.SaveChanges();

            return RedirectToPage("Index");
        }
    }
}