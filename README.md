# Simple Notification System (C# sample)

A small **.NET 8 console app** demonstrating a **3-tier design** with **CRUD + notifications** (Email/SMS/WhatsApp), organized as **multiple projects** under `SimpleNotificationSystem/`. Users and notifications persist in **PostgreSQL** via **Entity Framework Core** (Code First + migrations).

## What this repo contains

- **Projects** (no `.sln` file is checked in; run the Presentation project directly, or add your own solution file if you prefer):
  - `SimpleNotificationSystem/Presentation/` (console entry point)
  - `SimpleNotificationSystem/BusinessLayer/`
  - `SimpleNotificationSystem/DataAccessLayer/`
  - `SimpleNotificationSystem/NotificationSenders/`
  - `SimpleNotificationSystem/Interfaces/`
  - `SimpleNotificationSystem/Models/`

## How to run

Create the Postgres database once:

```bash
psql -U postgres -c "CREATE DATABASE notification_db;"
```

Update the connection string in `SimpleNotificationSystem/DataAccessLayer/DatabaseConfig.cs`, or set the `NOTIFICATION_DB` environment variable:

```bash
export NOTIFICATION_DB="Host=localhost;Port=5432;Database=notification_db;Username=postgres;Password=YOUR_PASSWORD"
```

From the repo root:

```bash
dotnet run --project SimpleNotificationSystem/Presentation
```

On startup, the app applies **EF Core migrations** (`Database.Migrate()`), which creates or updates the schema:

- `users`
- `notifications`

You’ll get a menu-driven CLI to:

- manage users (create/view/update/delete)
- create + send notifications (Email/SMS/WhatsApp)
- manage notifications (view/update/delete)

## Architecture 

The project demonstrates separation of concerns while still being small and runnable as a single console app.

### 3 tiers (plus shared projects)

Solution structure:

- **Presentation** (`SimpleNotificationSystem/Presentation/Program.cs`): CLI, input/output, menu flow
- **BusinessLayer** (`SimpleNotificationSystem/BusinessLayer/NotificationService.cs`): rules + orchestration
- **DataAccessLayer**:
  - `SimpleNotificationSystem/DataAccessLayer/DatabaseConfig.cs`: Postgres connection string and `DbContext` options (`UseNpgsql` + snake-case naming)
  - `SimpleNotificationSystem/DataAccessLayer/NotificationDbContext.cs`: EF Core Code First `DbContext`
  - `SimpleNotificationSystem/DataAccessLayer/NotificationDbContextFactory.cs`: design-time `DbContext` factory for `dotnet ef`
  - `SimpleNotificationSystem/DataAccessLayer/Migrations/`: EF Core migrations (schema as code)
  - `SimpleNotificationSystem/DataAccessLayer/UserRepository.cs`: user CRUD via EF Core
  - `SimpleNotificationSystem/DataAccessLayer/NotificationRepository.cs`: notification CRUD via EF Core
- **NotificationSenders** (`SimpleNotificationSystem/NotificationSenders/`*): channel implementations (Email/SMS/WhatsApp)
- **Models** (`SimpleNotificationSystem/Models/`*): domain models / enums
- **Interfaces** (`SimpleNotificationSystem/Interfaces/`*): abstractions (e.g., `INotificationSender`)

**Why this split?**

- Keeps the **UI** from owning business rules (validation, channel selection).
- Makes it easy to **swap senders** or persistence without rewriting the CLI.
- Mirrors common enterprise layering in a minimal, learnable form.

### Pluggable notification channels

Senders implement `INotificationSender`:

- `EmailNotificationSender`
- `SmsNotificationSender`
- `WhatsAppNotificationSender`

`NotificationService` selects the sender based on `NotificationType`.

**Why this approach?**

- Avoids `if/else` logic spread across the app.
- New channels (e.g., WhatsApp/Push) can be added with a new class implementing `INotificationSender`.

### Dependency wiring 

The CLI constructs:

- `NotificationDbContext` 
- `UserRepository(dbContext)`
- `NotificationRepository(dbContext)`
- `NotificationService(notificationRepo, emailSender, smsSender, whatsAppSender)`

### Persistence choice (PostgreSQL + EF Core)

Repositories persist through **Entity Framework Core** (Code First) to Postgres:

- `users`: user details with generated `id` and unique `email`
- `notifications`: notification rows with generated `id` and `user_id` foreign key (cascade delete)

**Why PostgreSQL + EF Core?**

- Data persists after the app exits.
- Schema is versioned with migrations instead of hand-written DDL.
- Recipient details come from the `User` navigation property; reads use `.Include(n => n.User)` where needed.

## Business rules captured

Implemented in `NotificationService`:

- **Message**: required, trimmed, at least 5 chars
- **SMS length**: max 160 characters
- **Email channel**: requires a valid email address
- **SMS/WhatsApp channel**: requires a phone number with 10–15 digits 

## Design decision notes

See:

- `docs/DECISIONS.md`
- `docs/ARCHITECTURE.md`

