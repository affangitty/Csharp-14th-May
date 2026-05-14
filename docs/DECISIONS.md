# Design decisions

## 1) 3-tier layering (split into multiple projects)

**Decision**: Keep Presentation/Business/DataAccess as separate layers, and split them into separate projects in the solution.

**Why**:

- Still demonstrates separation of concerns and layering.
- Enforces boundaries more clearly than folders alone.
- Keeping the projects under `SimpleNotificationSystem/` groups the domain into a single “module” folder while preserving clean layer boundaries.

**Trade-off**:

- More projects means more files and references to manage for a small demo.

## 2) Manual dependency wiring 

**Decision**: Construct dependencies directly in `Program.cs`.

**Why**:

- Fewer concepts for a learning repo.
- Dependencies remain explicit.

**Trade-off**:

- As the app grows, composition can get noisy. A DI container (Microsoft.Extensions.DependencyInjection) would scale better.

## 3) `INotificationSender` for channel implementations

**Decision**: Use an interface for notification senders and select implementation based on `NotificationType`.

**Why**:

- Demonstrates Strategy-style extensibility (add a new sender with minimal change).
- Keeps sender logic out of the business service.

**Trade-off**:

- The selection logic still lives in `NotificationService`. A more scalable approach would use a map/registry keyed by `NotificationType`.

## 4) PostgreSQL storage for notifications

**Decision**: `NotificationRepository` persists notifications through **EF Core** to the `notifications` table in PostgreSQL.

**Why**:

- Data persists after the app exits.
- The data tier demonstrates real persistence with a mainstream ORM and migrations.
- Notification rows reference users through `user_id` (foreign key).

**Trade-off**:

- Requires a running Postgres server and a valid connection string.

## 4b) PostgreSQL storage for users

**Decision**: `UserRepository` persists users through **EF Core** to the `users` table in PostgreSQL. The database uses a generated numeric `id`, while email remains unique and is still used by the CLI for lookup.

**Why**:

- Email is easy to type/search in a CLI and acts as a natural unique key for a demo.
- A generated numeric `id` is safer for relationships between tables.
- `notifications.user_id` can reference `users.id` with a foreign key.

**Trade-off**:

- The CLI still asks for email when selecting users because it is easier to enter than an internal database id.

## 4c) Related user data on notifications

**Decision**: Notifications reference users by `UserId` and expose a `User` navigation property. When the CLI needs recipient details, `NotificationRepository` loads the related user with EF Core (for example `.Include(n => n.User)` or an explicit reference load after insert).

**Why**:

- Notification rows only store `user_id`, avoiding duplicated user details in the database.
- The CLI can still display recipient name, email, and phone number with the notification.
- EF Core translates the relationship load to an efficient SQL join under the hood.

**Trade-off**:

- Care must be taken to include or load navigation properties where needed; otherwise the `User` property may be null on detached graphs.

## 5) Simple validation rules

**Decision**: Validate message and user contact info in `NotificationService`.

**Why**:

- Centralizes business rules in one place (single source of truth).
- Prevents invalid states from being “sent” or saved.

**Trade-off**:

- Validation is simplified. Production code would use stricter rules and better localization/format handling.

## 6) Adding WhatsApp as a third channel

**Decision**: Add `NotificationType.WhatsApp` and implement `WhatsAppNotificationSender`.

**Why**:

- Demonstrates how the `INotificationSender` abstraction scales beyond Email/SMS.
- Keeps the “send” mechanism consistent while extending the domain.

