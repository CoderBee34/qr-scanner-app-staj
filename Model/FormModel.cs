using System.ComponentModel.DataAnnotations;

namespace qr_scanner_app_staj.Model
{
    public class FormModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string RePassword { get; set; }
    }
}