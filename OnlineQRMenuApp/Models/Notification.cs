using System.ComponentModel.DataAnnotations;

namespace OnlineQRMenuApp.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        [Required]
        public int UserId { get; set; }
        public string Message { get; set; }
        public DateTime SentDate { get; set; }
        public bool IsRead { get; set; }
        public virtual User User { get; set; }
    }
}
