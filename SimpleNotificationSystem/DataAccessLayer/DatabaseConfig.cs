using Microsoft.EntityFrameworkCore;

namespace SimpleNotificationSystem.DataAccessLayer;

public static class DatabaseConfig
{
    public static string ConnectionString { get; set; } =
        Environment.GetEnvironmentVariable("NOTIFICATION_DB")
        ?? "Host=localhost;Port=5432;Database=notification_db;Username=postgres;Password=affuraja";

    public static DbContextOptions<NotificationDbContext> CreateDbContextOptions() =>
        new DbContextOptionsBuilder<NotificationDbContext>()
            .UseNpgsql(ConnectionString)
            .UseSnakeCaseNamingConvention()
            .Options;
}
