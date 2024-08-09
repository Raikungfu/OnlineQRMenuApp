using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineQRMenuApp.Models
{
    public class MenuItemCustomization
    {
        [Key]
        public int MenuItemCustomizationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Additional cost must be a positive value")]
        public decimal AdditionalCost { get; set; } = 0;
        [Required]
        public int CustomizationGroupId { get; set; }
    }
}