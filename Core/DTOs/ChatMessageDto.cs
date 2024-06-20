using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.DTOs
{
    public class ChatMessageDto
    {
        public string? SenderUserId { get; set; }
        public string? ReceiverUserId { get; set; }
        public string? UserName { get; set; }
        public string? Content { get; set; }
        public string? MediaUrl { get; set; } 
        [NotMapped]
        public int? ChatGroupId { get; set; }
        public string? ChatGroup { get; set; }

        public IFormFile? MediaFile { get; set; }
        public DateTime Timestamp { get; set; }
        public string? AppUserId { get; set; } 
        public string? AppUser { get; set; } 
    }
}
