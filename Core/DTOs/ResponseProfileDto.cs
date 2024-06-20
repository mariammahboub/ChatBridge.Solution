namespace Core.DTOs
{
    public class ResponseProfileDto
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public string Pio { get; set; }
        public string ProfilePictureUrl { get; set; } 
        public ResponseProfileDto()
        {
        }
    }
}
