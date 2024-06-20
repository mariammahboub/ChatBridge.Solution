using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class ChatGroupDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? AdminUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string>? MemberUserId { get; set; }
        public string? GroupMember { get; set; }
        public List<string>? MemberUsernames { get; set; }
    }
}
