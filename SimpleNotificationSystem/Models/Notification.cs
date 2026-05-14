namespace SimpleNotificationSystem.Models;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public NotificationType Type { get; set; }

    public string Message { get; set; } = string.Empty;
    public DateTime SentDate { get; set; }

    public User User { get; set; } = null!;
}
