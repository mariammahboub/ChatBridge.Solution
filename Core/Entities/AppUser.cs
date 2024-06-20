using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Core.Entities
{
    public class AppUser : IdentityUser
    {
        public Profile Profile { get; set; } 

        public ICollection<Friendship> Friendships { get; set; } 

        public ICollection<FriendRequest> FriendRequests { get; set; } 
        public ICollection<ChatMessage> ChatMessages { get; set; } 
        public ICollection<GroupMember> GroupMembers { get; set; } 



        public AppUser()
        {
            FriendRequests = new HashSet<FriendRequest>();
            Friendships = new HashSet<Friendship>();
            ChatMessages = new HashSet<ChatMessage>();
            GroupMembers = new HashSet<GroupMember>();

        }
    }
}
