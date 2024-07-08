using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace qr_scanner_app_staj.Pages.qr_scanner
{
    public class ScannerModel() : PageModel
    {
        [BindProperty]
        public string QrCode { get; set; }
        public async Task<IActionResult> OnPost()
        {
            Console.WriteLine(QrCode);
            return RedirectToPage("Receipts");
        }
    }
}