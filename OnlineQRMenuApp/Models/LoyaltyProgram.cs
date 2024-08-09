using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineQRMenuApp.Models
{
    public class LoyaltyProgram
    {
        [Key]
        public int LoyaltyProgramId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Points must be a non-negative value")]
        public int Points { get; set; } = 0;

        [Required]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
