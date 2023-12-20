using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PinedaApp.Models
{
    public class Academic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? SchoolName { get; set; }
        public string? Degree { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public int UserId { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }
    }
}
