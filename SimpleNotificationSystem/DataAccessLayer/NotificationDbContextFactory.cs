using Microsoft.EntityFrameworkCore.Design;

namespace SimpleNotificationSystem.DataAccessLayer;

public class NotificationDbContextFactory : IDesignTimeDbContextFactory<NotificationDbContext>
{
    public NotificationDbContext CreateDbContext(string[] args) =>
        new(DatabaseConfig.CreateDbContextOptions());
}
