namespace PinedaApp.Models.DTO
{
    public class ExpertiseDto
    {
        public int Id { get; set; }
        public int UserId {  get; set; }
        public string Skills { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
