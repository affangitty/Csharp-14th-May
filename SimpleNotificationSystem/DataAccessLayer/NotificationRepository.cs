using Microsoft.EntityFrameworkCore;
using SimpleNotificationSystem.Models;

namespace SimpleNotificationSystem.DataAccessLayer;

public class NotificationRepository
{
    private readonly NotificationDbContext _context;

    public NotificationRepository(NotificationDbContext context) =>
        _context = context;

    public Notification Save(Notification notification)
    {
        _context.Notifications.Add(notification);
        _context.SaveChanges();

        _context.Entry(notification).Reference(n => n.User).Load();
        return notification;
    }

    public IReadOnlyList<Notification> GetAll() =>
        _context.Notifications
            .AsNoTracking()
            .Include(n => n.User)
            .OrderBy(n => n.Id)
            .ToList()
            .AsReadOnly();

    public Notification? GetById(int id) =>
        _context.Notifications
            .AsNoTracking()
            .Include(n => n.User)
            .FirstOrDefault(n => n.Id == id);

    public List<int> GetIds() =>
        _context.Notifications
            .AsNoTracking()
            .OrderBy(n => n.Id)
            .Select(n => n.Id)
            .ToList();

    public Notification? Update(int id, Notification updated)
    {
        var entity = _context.Notifications.FirstOrDefault(n => n.Id == id);
        if (entity is null) return null;

        entity.Message = updated.Message;
        _context.SaveChanges();

        return GetById(id);
    }

    public Notification? Delete(int id)
    {
        var existing = GetById(id);
        if (existing is null) return null;

        _context.Notifications.Remove(_context.Notifications.Single(n => n.Id == id));
        _context.SaveChanges();

        return existing;
    }
}
