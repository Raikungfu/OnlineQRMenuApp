using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineQRMenuApp.Models
{
    public class QRCode
    {
        [Key]
        public int QRCodeId { get; set; }

        public string Link { get; set; } = string.Empty;

        public int tableId { get; set; }

        [Required]
        public int CoffeeShopId { get; set; }
        [ForeignKey("CoffeeShopId")]
        public virtual CoffeeShop CoffeeShop { get; set; }
    }
}
