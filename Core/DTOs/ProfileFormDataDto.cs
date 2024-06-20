using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.DTOs
{
    public class ProfileFormDataDto
    {
        public string? ProfilePicture { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public string? Pio { get; set; }
        public int? AppUserId { get; set; }
        public string? AppUser { get; set; }
    }
}
