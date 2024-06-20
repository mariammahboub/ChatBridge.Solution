namespace Core.Entities
{
    public class FriendRequest: BaseEntity
    {
        public string SenderUserId { get; set; }
        public string ReceiverUserId { get; set; }
        public string AppUserId { get; set; } 
        public AppUser AppUser { get; set; }
        public FriendRequestStatus Status { get; set; }
    }

    public enum FriendRequestStatus
    {
        Pending=1,
        Accepted=2,
        Rejected=3,
        Blocked = 4

    }
}
