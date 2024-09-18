using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineQRMenuApp.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int MenuItemId { get; set; }

        public string Note { get; set; }

        public string SizeOptions { get; set; }

        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
        public decimal Price { get; set; } = 0.0m;

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
        public decimal Cost { get; set; } = 0.0m;

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        [ForeignKey("MenuItemId")]
        public virtual MenuItem MenuItem { get; set; }
    }
}