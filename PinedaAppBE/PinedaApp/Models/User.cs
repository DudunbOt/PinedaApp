using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PinedaApp.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Bio { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Occupation { get; set; }
        public Role? UserRole { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public virtual ICollection<Academic>? Academics { get; set; }
        public virtual ICollection<Experience>? Experiences { get; set; }
        public virtual ICollection<Portfolio>? Portfolios { get; set; }
        public virtual Expertise Expertise { get; set; }
    }

    public enum Role
    {
        Owner,
        User
    }
}
