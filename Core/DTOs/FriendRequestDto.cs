using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class FriendRequestDto
    {
        public string? Username { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Pio { get; set; }
        public int? RequestId { get; set; }
        public string? SenderId { get; set; }
        public string? ReceiverId { get; set; }
        public string? AppUserId { get; set; }

        public FriendRequestStatus? Status { get; set; }

    }
}
