using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineQRMenuApp.Models
{
    public class OrderItemCustomization
    {
        [Key]
        public int OrderItemCustomizationId { get; set; }
        [Required]
        public int OrderItemId { get; set; }
        [Required]
        public int MenuItemCustomizationId { get; set; }

        [ForeignKey("OrderItemId")]
        public virtual OrderItem OrderItem { get; set; }
        [ForeignKey("MenuItemCustomizationId")]
        public virtual MenuItemCustomization MenuItemCustomization { get; set; }
    }
}
