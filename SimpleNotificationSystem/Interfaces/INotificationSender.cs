using SimpleNotificationSystem.Models;

namespace SimpleNotificationSystem.Interfaces;

public interface INotificationSender
{
    void Send(User user, Notification notification);
}

