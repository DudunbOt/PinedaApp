using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PinedaApp.Models
{
    public class ProjectHandled
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectDescription { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }

        public int ExperienceId { get; set; }
        [JsonIgnore]
        public virtual Experience Experience { get; set; }
    }
}
