using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Utilities.IO.Pem;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class ChatMessage : BaseEntity
    {
        public string SenderUserId { get; set; }
        public string ReceiverUserId { get; set; }
        public string Content { get; set; }
        public string? MediaUrl { get; set; } 
        [NotMapped]
        public IFormFile? MediaFile { get; set; } 

        public DateTime Timestamp { get; set; }
        public int? ChatGroupId { get; set; }
        public ChatGroup? ChatGroup    { get; set; }
        public string? AppUserId { get; set; } 
        public AppUser? AppUser { get; set; } 
        public bool? IsArchived { get; set; } 

    }
}
