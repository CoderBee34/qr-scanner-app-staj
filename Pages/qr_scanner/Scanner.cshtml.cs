using System.Diagnostics;
using IronOcr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
namespace qr_scanner_app_staj.Pages.qr_scanner
{
    public class ScannerModel() : PageModel
    {
        [BindProperty]
        public string QrCode { get; set; }
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