namespace PinedaApp.Models.DTO
{
    public class ExperienceDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string ShortDesc {  get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public List<ProjectHandledDto> Projects { get; set; }
    }
}
