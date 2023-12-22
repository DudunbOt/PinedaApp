using Microsoft.EntityFrameworkCore;
using PinedaApp.Models;

namespace PinedaApp.Configurations
{
    public class PinedaAppContext : DbContext
    {
        public PinedaAppContext(DbContextOptions<PinedaAppContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Academic> Academic { get; set; }
        public DbSet<Portfolio> Portfolio { get; set; }
        public DbSet<Experience> Experience { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<Expertise> Expertise { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<TransactionCategory> TransactionCategory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLazyLoadingProxies().UseSqlServer(@"
                Data Source=localhost;
                Initial Catalog=PinedaApp;
                User Id=Pineda;
                Password=qwerasdf123;
                TrustServerCertificate=True;
            ");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<User>()
                .HasMany(p => p.Academics)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);

            modelBuilder.Entity<User>()
                .HasMany(p => p.Experiences)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<User>()
                .HasMany(p => p.Portfolios)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<User>()
                .HasOne(p => p.Expertise)
                .WithOne(f => f.User)
                .HasForeignKey<Expertise>(f => f.UserId);

            modelBuilder.Entity<Experience>()
                .HasMany(e => e.Project)
                .WithOne(p => p.Experience)
                .HasForeignKey(p => p.ExperienceId);

            modelBuilder.Entity<Experience>()
                .Property(e => e.EndDate)
                .IsRequired(false);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Transactions)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<TransactionCategory>()
                .HasMany(tc => tc.Transactions)
                .WithOne(t => t.Category)
                .HasForeignKey(t => t.CategoryId);
            
        }
    }
}