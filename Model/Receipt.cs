using System.ComponentModel.DataAnnotations;

namespace qr_scanner_app_staj.Model
{
    public class Receipt
    {
        [Key]
        public int receiptId { get; set; }

        public string date { get; set; }
        public double total { get; set; }
        public double totalTax { get; set; }
        public int userId { get; set; }
    }
}