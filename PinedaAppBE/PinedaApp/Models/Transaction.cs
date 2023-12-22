using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PinedaApp.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public string Name { get; set; }
        public double Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public int CategoryId { get; set; }
        [JsonIgnore]
        public virtual TransactionCategory Category { get; set; }

        public int? BudgetId { get; set; }
        [JsonIgnore]
        public virtual Budget Budget { get; set; }

        public int UserId { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }
    }
}
