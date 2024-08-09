using System.ComponentModel.DataAnnotations;

namespace OnlineQRMenuApp.Models
{
    public class CustomizationGroup
    {
        public int CustomizationGroupId { get; set; }
        public string Name { get; set; } = string.Empty;

        [Required]
        public int MenuItemId { get; set; }
        public virtual ICollection<MenuItemCustomization> Customizations { get; set; }
    }
}