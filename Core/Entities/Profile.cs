using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Profile: BaseEntity
    {
        public string? ProfilePicture { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public string? Pio { get; set; }
        public string? AppUserId { get; set; } 
        public AppUser? AppUser { get; set; } 
    }
}
