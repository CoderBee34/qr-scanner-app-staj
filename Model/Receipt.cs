using System.ComponentModel.DataAnnotations;

namespace qr_scanner_app_staj.Model
{
    public class Receipt
    {
        public Receipt(int receiptId, string date, double total, double totalTax, int userId)
        {
            this.receiptId = receiptId;
            this.date = date;
            this.total = total;
            this.totalTax = totalTax;
            this.userId = userId;
        }
        [Key]
        public int receiptId { get; set; }

        public string date { get; set; }
        public double total { get; set; }
        public double totalTax { get; set; }
        public int userId { get; set; }
    }
}