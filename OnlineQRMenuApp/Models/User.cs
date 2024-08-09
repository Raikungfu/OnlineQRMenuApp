using System.ComponentModel.DataAnnotations;

namespace OnlineQRMenuApp.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string Address { get; set; }
        public string UserType { get; set; }

        public virtual ICollection<LoyaltyProgram> LoyaltyPrograms { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }

}
