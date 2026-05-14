namespace SimpleNotificationSystem.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
