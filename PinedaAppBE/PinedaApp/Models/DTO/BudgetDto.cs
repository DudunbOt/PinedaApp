using System.ComponentModel.DataAnnotations.Schema;

namespace PinedaApp.Models.DTO
{
    public class BudgetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Goal { get; set; }
        public double Current { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public double Remaining
        {
            get
            {
                if (Current >= Goal) return 0;
                return Goal - Current;
            }
        }
    }
}
