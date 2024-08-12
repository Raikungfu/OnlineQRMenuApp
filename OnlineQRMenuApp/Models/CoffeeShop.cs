using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineQRMenuApp.Models
{
    public class CoffeeShop
    {
        [Key]
        public int CoffeeShopId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string QRCode { get; set; } = string.Empty;

        public string PrimaryColor { get; set; } = "#000000";

        public string SecondaryColor { get; set; } = "#FFFFFF";

        public string Description { get; set; }

        public string Slogan { get; set; }

        public string Avatar { get; set; }

        public string CoverImage { get; set; }

        public string Hotline { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }

        public string Facebook { get; set; }

        public string Instagram { get; set; }

        public string Twitter { get; set; }

        public string OpeningHours { get; set; } = "08:00 AM - 10:00 PM";

        public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public virtual ICollection<CoffeeShopCustomer> CoffeeShopCustomers { get; set; } = new List<CoffeeShopCustomer>();

    }
}
