# Architecture overview

## High-level flow

1. **Presentation** gathers inputs for **User CRUD**, **Notification CRUD**, and “create + send notification”.
2. **BusinessLayer** validates inputs, builds a `Notification`, chooses the correct sender, and persists the notification.
3. **Notification sender** prints a simulated “send” action to the console.
4. **Repositories** store users + notifications in PostgreSQL via **EF Core** so the CLI can query/update/delete persistent data.

## Components

### Presentation

- `SimpleNotificationSystem/Presentation/Program.cs`
- Owns the console menu, input loops, and printing.

**Important boundary**: the CLI should not embed rules like “SMS <= 160 chars” or “email must be valid”; those belong in the business layer.

### Business layer

- `SimpleNotificationSystem/BusinessLayer/NotificationService.cs`

Responsibilities:

- validate message and recipient data
- build a `Notification` domain object
- select an `INotificationSender` based on `NotificationType`
- persist the `Notification` via `NotificationRepository`

### Data access layer

- `SimpleNotificationSystem/DataAccessLayer/DatabaseConfig.cs`
- `SimpleNotificationSystem/DataAccessLayer/NotificationDbContext.cs`
- `SimpleNotificationSystem/DataAccessLayer/NotificationDbContextFactory.cs` 
- `SimpleNotificationSystem/DataAccessLayer/Migrations/` (Code First migrations)
- `SimpleNotificationSystem/DataAccessLayer/NotificationRepository.cs`
- `SimpleNotificationSystem/DataAccessLayer/UserRepository.cs`

Responsibilities:

- **DatabaseConfig**: central Postgres connection string and `DbContextOptions` .
- **NotificationDbContext**: EF Core model (users, notifications, relationship, unique email, cascade delete).
- **UserRepository** / **NotificationRepository**: CRUD using LINQ and `SaveChanges`; notification reads load the related `User` with `.Include()` where recipient details are needed.

The relational schema is defined by EF Core migrations (see `Migrations/`). At runtime, `Program.cs` calls `Database.Migrate()` to apply pending migrations.

### Notification senders

- `SimpleNotificationSystem/Interfaces/INotificationSender.cs`
- `SimpleNotificationSystem/NotificationSenders/EmailNotificationSender.cs`
- `SimpleNotificationSystem/NotificationSenders/SmsNotificationSender.cs`
- `SimpleNotificationSystem/NotificationSenders/WhatsAppNotificationSender.cs`

