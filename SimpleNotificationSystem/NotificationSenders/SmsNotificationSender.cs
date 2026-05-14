using SimpleNotificationSystem.Interfaces;
using SimpleNotificationSystem.Models;

namespace SimpleNotificationSystem.NotificationSenders;

public class SmsNotificationSender : INotificationSender
{
    public void Send(User user, Notification notification)
    {
        Console.WriteLine(
            $"[SMS] To: {user.PhoneNumber} | Message: {notification.Message} | Sent: {notification.SentDate:yyyy-MM-dd HH:mm:ss}");
    }
}

