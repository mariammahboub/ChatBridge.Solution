using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Friendship: BaseEntity
    {
        public string User1Id { get; set; } 
        public string User2Id { get; set; } 

        [ForeignKey("User1Id")]
        public AppUser User1 { get; set; }

        [ForeignKey("User2Id")]
        public AppUser User2 { get; set; } 
        public FriendshipStatus Status { get; set; } 
        public enum FriendshipStatus
        {
            Pending = 1,
            Accepted = 2,
            Rejected = 3,
            Blocked = 4
        }

    }
}
