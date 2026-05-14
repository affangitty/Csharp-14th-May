using SimpleNotificationSystem.Interfaces;
using SimpleNotificationSystem.Models;

namespace SimpleNotificationSystem.NotificationSenders;

public class WhatsAppNotificationSender : INotificationSender
{
    public void Send(User user, Notification notification)
    {
        Console.WriteLine(
            $"[WhatsApp] To: {user.PhoneNumber} | Message: {notification.Message} | Sent: {notification.SentDate:yyyy-MM-dd HH:mm:ss}");
    }
}

