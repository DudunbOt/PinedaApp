using PinedaApp.Models.Errors;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinedaApp.Models
{
    public class TransactionCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type {  get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public int TransactionId { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }

        [NotMapped]
        public TransactionType GetType
        {
            get
            {
                if (Enum.TryParse(Type, true, out TransactionType result)) return result;

                throw new PinedaAppException("Invalid Transaction Type", 404);
            }
        }
    }

    public enum TransactionType
    {
        Income,
        Outcome,
        Saving
    }
}
