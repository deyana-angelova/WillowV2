using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WillowProject.Models;
using WillowV2.Models;

namespace WillowV2.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<FinTransaction> FinTransactions { get; set; } //transactions data
        public DbSet<FinCategory> FinCategories { get; set; } //category data
        public DbSet<Goal> Goals { get; set; } //goals data
        public DbSet<Savings> Savings { get; set; } //savings data
        public DbSet<RecurringTransaction> RecurringTransactions { get; set; } //reccuring transactions
        public DbSet<Split> Splits { get; set; }
        public DbSet<SplitParticipant> SplitParticipants { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FinCategory>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Goal>()
                .HasOne(g => g.User)
                .WithMany()
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Savings>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Split>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
