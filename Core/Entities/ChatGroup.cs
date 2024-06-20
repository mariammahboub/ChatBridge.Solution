using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ChatGroup:BaseEntity
    {
        public string Name { get; set; }
        public string AdminUserId { get; set; } 
        public DateTime CreatedAt { get; set; }

        public ICollection<GroupMember> GroupMember { get; set; }
        public ICollection<ChatMessage> ChatMessage { get; set; }
        public bool? IsArchived { get; set; } 


    }
}
