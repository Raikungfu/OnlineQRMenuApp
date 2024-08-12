using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineQRMenuApp.Models
{
    public class CoffeeShopCustomer
    {
        [Key]
        public int CoffeeShopCustomerId { get; set; }

        [Required]
        public int CoffeeShopId { get; set; }
        [ForeignKey("CoffeeShopId")]
        public virtual CoffeeShop CoffeeShop { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public DateTime JoinedDate { get; set; }
    }

}
