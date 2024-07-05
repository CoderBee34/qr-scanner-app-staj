using System.ComponentModel.DataAnnotations;

namespace qr_scanner_app_staj.Model
{
    public class Receipt
    {
        [Key]
        public int receiptId { get; set; }

        public string date { get; set; }
        public int total { get; set; }
        public int totalTax { get; set; }
    }
}