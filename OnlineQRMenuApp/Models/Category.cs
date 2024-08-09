using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineQRMenuApp.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<MenuItem> MenuItems { get; set; }
    }
}
