using System.ComponentModel.DataAnnotations;

namespace qr_scanner_app_staj.Model
{
    public class User
    {
        [Key]
        public int userId { get; set; }

        [Required]
        public string username { get; set; }
        public string password { get; set; }
    }
}