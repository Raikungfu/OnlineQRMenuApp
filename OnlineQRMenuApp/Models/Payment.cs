using System.ComponentModel.DataAnnotations;

namespace OnlineQRMenuApp.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        [Required]
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; }

        public virtual Order Order { get; set; }
    }
}
