using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OnlineQRMenuApp.Models
{
    public class MenuItem
    {
        [Key]
        public int MenuItemId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
        public decimal Price { get; set; } = 0.0m;

        public string Size { get; set; }

        public string Type { get; set; }

        [Required]
        public int CoffeeShopId { get; set; }

        [JsonIgnore]
        [ForeignKey("CoffeeShopId")]
        public virtual CoffeeShop CoffeeShop { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [JsonIgnore]
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public string Options { get; set; }

        [JsonIgnore]
        public virtual ICollection<CustomizationGroup> CustomizationGroups { get; set; } = new List<CustomizationGroup>();

        [JsonIgnore]
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
