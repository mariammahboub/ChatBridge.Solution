using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class GroupMember:BaseEntity
    {
        public int ChatGroupId { get; set; }
        public string AppUserId { get; set; }

        public ChatGroup ChatGroup { get; set; }
        public AppUser AppUser { get; set; }
    }
}
