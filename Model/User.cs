using System.ComponentModel.DataAnnotations;

namespace qr_scanner_app_staj.Model
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string username { get; set; }
        public string password { get; set; }
    }
}