using Microsoft.EntityFrameworkCore;
using SimpleNotificationSystem.Models;

namespace SimpleNotificationSystem.DataAccessLayer;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
            entity.Property(e => e.PhoneNumber).HasMaxLength(50).IsRequired();

            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasMany(e => e.Notifications)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("notifications");

            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.SentDate).IsRequired();

            entity.Property(e => e.Type)
                .HasConversion<int>()
                .IsRequired();
        });
    }
}
