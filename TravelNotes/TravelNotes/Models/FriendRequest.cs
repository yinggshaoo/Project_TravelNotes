using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelNotes.Models
{
    public class FriendRequest
    {
        [Key]
        public int uuid { get; set; }
        public int SenderUserId { get; set; }
        public int ReceiverUserId { get; set; }

        // 0 pending, 1 accept, -1 reject
        public int Status { get; set; }
        [ForeignKey("SenderUserId")]
        public users? SenderUser { get; set; }
        [ForeignKey("ReceiverUserId")]
        public users? ReceiverUser { get; set; }
    }
}
