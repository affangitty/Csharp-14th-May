using System.Net.Mail;
using SimpleNotificationSystem.DataAccessLayer;
using SimpleNotificationSystem.Interfaces;
using SimpleNotificationSystem.Models;

namespace SimpleNotificationSystem.BusinessLayer;

public class NotificationService
{
    private readonly NotificationRepository _repository;
    private readonly INotificationSender _emailSender;
    private readonly INotificationSender _smsSender;
    private readonly INotificationSender _whatsAppSender;

    public NotificationService(
        NotificationRepository repository,
        INotificationSender emailSender,
        INotificationSender smsSender,
        INotificationSender whatsAppSender)
    {
        _repository = repository;
        _emailSender = emailSender;
        _smsSender = smsSender;
        _whatsAppSender = whatsAppSender;
    }

    public Notification Send(User user, NotificationType type, string message)
    {
        ValidateMessageOrThrow(type, message);
        ValidateUserForChannel(type, user);

        var notification = new Notification
        {
            UserId = user.Id,
            Type = type,
            Message = message.Trim(),
            SentDate = DateTime.UtcNow
        };

        GetSender(type).Send(user, notification);
        return _repository.Save(notification);
    }

    public void ValidateMessage(NotificationType type, string message) =>
        ValidateMessageOrThrow(type, message);

    private INotificationSender GetSender(NotificationType type) =>
        type switch
        {
            NotificationType.Email => _emailSender,
            NotificationType.Sms => _smsSender,
            NotificationType.WhatsApp => _whatsAppSender,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported notification type.")
        };

    private static void ValidateMessageOrThrow(NotificationType type, string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message should not be empty.");

        var trimmed = message.Trim();
        if (trimmed.Length < 5)
            throw new ArgumentException("Message length should be at least 5 characters.");

        if (type == NotificationType.Sms && trimmed.Length > 160)
            throw new ArgumentException("SMS message should not exceed 160 characters.");
    }

    private static void ValidateUserForChannel(NotificationType type, User user)
    {
        if (user is null) throw new ArgumentNullException(nameof(user));

        if (type == NotificationType.Email && !IsValidEmail(user.Email))
            throw new ArgumentException("Email notification requires a valid email address.");

        if (type is NotificationType.Sms or NotificationType.WhatsApp)
        {
            if (!IsValidPhoneNumber(user.PhoneNumber))
                throw new ArgumentException($"{type} notification requires a valid phone number.");
        }
    }

    private static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        try
        {
            _ = new MailAddress(email.Trim());
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidPhoneNumber(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return false;

        var digits = phone.Where(char.IsDigit).ToArray();
        return digits.Length is >= 10 and <= 15;
    }
}

