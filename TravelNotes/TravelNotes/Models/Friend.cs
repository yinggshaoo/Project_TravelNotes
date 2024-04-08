using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelNotes.Models
{
    public class Friend
    {
        [Key]
        public int uuid { get; set; }
        public int UserId { get; set; }
        public int FriendId { get; set; }
        public bool Status { get; set; }
        public users? User { get; set; }
        [ForeignKey("FriendId")]
        public users? FriendUser { get; set; }
    }
}
