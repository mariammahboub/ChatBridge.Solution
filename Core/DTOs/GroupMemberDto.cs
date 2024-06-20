using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class GroupMemberDto
    {
        public int Id { get; set; }
        public int ChatGroupId { get; set; }
        public string AppUserId { get; set; }
    }
}
