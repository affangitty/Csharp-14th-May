using Microsoft.EntityFrameworkCore;
using SimpleNotificationSystem.Models;

namespace SimpleNotificationSystem.DataAccessLayer;

public class UserRepository
{
    private readonly NotificationDbContext _context;

    public UserRepository(NotificationDbContext context) =>
        _context = context;

    public User Create(User user)
    {
        if (user is null) throw new ArgumentNullException(nameof(user));

        _context.Users.Add(user);
        _context.SaveChanges();
        return user;
    }

    public User? GetByEmail(string email) =>
        _context.Users
            .AsNoTracking()
            .FirstOrDefault(u => u.Email == email);

    public List<User> GetAll() =>
        _context.Users
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .ToList();

    public User? Update(string email, User user)
    {
        var entity = _context.Users.FirstOrDefault(u => u.Email == email);
        if (entity is null) return null;

        entity.Name = user.Name;
        entity.PhoneNumber = user.PhoneNumber;
        _context.SaveChanges();
        return entity;
    }

    public User? Delete(string email)
    {
        var entity = _context.Users.FirstOrDefault(u => u.Email == email);
        if (entity is null) return null;

        _context.Users.Remove(entity);
        _context.SaveChanges();
        return entity;
    }
}
