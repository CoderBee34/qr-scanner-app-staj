using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
namespace qr_scanner_app_staj.Pages.qr_scanner
{
    public class ScannerModel() : PageModel
    {
        [BindProperty]
        public string QrCode { get; set; }
        public async Task<IActionResult> OnPost()
        {
            Console.WriteLine(QrCode);
            IWebDriver driver = new ChromeDriver();
            try
            {
                driver.Navigate().GoToUrl(QrCode);
                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();

                string filePath = Path.Combine(Environment.CurrentDirectory, "screenshot.png");
                screenshot.SaveAsFile(filePath);

                Console.WriteLine($"Screenshot saved to: {filePath}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }
            finally
            {
                driver.Quit();
            }
            return RedirectToPage("Receipts");
        }
    }
}