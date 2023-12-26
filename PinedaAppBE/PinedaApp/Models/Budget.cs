using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PinedaApp.Models
{
    public class Budget
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Goal { get; set; }
        public double Current { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set;}
        
        [NotMapped]
        public double Remaining { 
            get
            {
                if (Current >= Goal) return 0;
                return Goal - Current;
            }
        }

        public int UserId {  get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }

        [JsonIgnore]
        public virtual List<Transaction> Transactions { get; set; }
    }
}
