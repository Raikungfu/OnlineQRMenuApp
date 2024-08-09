using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineQRMenuApp.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int CoffeeShopId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("CoffeeShopId")]
        public virtual CoffeeShop CoffeeShop { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
