# Notification center and interface motion

## UI

The old `ngx-toastr` dependency and green rectangle toasts were removed. `ToastService` and the separate TS/HTML/CSS `ToastCenterComponent` provide a neutral card, status icon, translated title/message, dismiss button and timed progress line. Toasts use polite live announcements; error messages use alert semantics. Success lasts 4.5 seconds and errors 8 seconds. Important inline form/API errors remain on the screen; the toast is not the only way to see them.

Motion is intentionally short: page/card entrance, staggered dashboard cards, navigation drawer, button press/hover and a limited unread-bell nudge. It does not delay API actions. `prefers-reduced-motion` removes animation/transition effects. Keyboard focus is visible.

The notification center provides:

- All updates, appointments, reminders and general filters, applied server-side before pagination.
- Unread-only and separate hidden-notification views.
- Mark read, mark all visible notifications read, hide, restore and confirmed permanent delete.
- An unread badge in the header, refreshed on account load and notification changes. This is not a live push/SignalR feed; a new notification during a long session appears when the center is opened/refreshed or the next relevant refresh runs.
- Responsive Arabic/English layouts and account delivery preferences for appointment updates, appointment reminders and general updates.

The Facebook reference was not accessible. This is an independent implementation of the requested behavior, not a claim to copy that unseen design.

## API and migration

Included follow-up migration: `20261123212621_NotificationVisibility`.

- `Notifications.HiddenAtUtc`, nullable datetime2. Null means visible.
- `NotificationPreferences.AppointmentUpdates`, bit, defaults to true for existing accounts.
- The migration snapshot and generated idempotent `MIGRATIONS.sql` include both this change and the bilingual migration.

Apply using the same instructions in BILINGUAL-SETUP.md:

```sh
dotnet tool restore
dotnet ef database update --project TourSite.Repository --startup-project TourSite.APIs
dotnet run --project TourSite.APIs
```

Development startup also applies pending migrations and seed. Do not generate the included migration again.

New/read routes:

```text
GET    /api/Notifications?category=all&hidden=false&unreadOnly=false&pageIndex=1&pageSize=10
PUT    /api/Notifications/{id}/visibility?hidden=true
PUT    /api/Notifications/{id}/visibility?hidden=false
DELETE /api/Notifications/{id}
```

Supported category values: `all`, `appointments`, `reminders`, `general`. Unknown values produce 400. `AppointmentReminder` is a reminder; other `Appointment*` types are appointments; other types are general. Every read or write is scoped to the authenticated account. Other users cannot hide, restore or delete a record. Hidden records are excluded from the default list and unread badge, but remain recoverable from the hidden view. Delete is permanent.

Existing preference routes now accept `appointmentUpdates` alongside `appointmentReminders` and `generalUpdates`. These category switches suppress creation of *new* matching notifications. They do not delete existing history. Channel switches still depend on configured external providers; no email/SMS/push/WhatsApp provider is set up by this work.

Notification sending no longer uses an unawaited task holding a request-scoped DbContext. It is awaited within the operation and records sent/failed status. A production system should use a durable outbox/queue for provider retries and operational guarantees. The pre-existing preference-update bug that marked a newly added preference as Modified was fixed.

## Verification

- 36 browser tests pass, including API request shapes for filters/hide/restore/delete, Arabic narrow layout, reduced motion and toast dismissal.
- 13 API-host tests pass, including notification ownership, filtering/visibility/delete and suppressed categories.
- Tests use browser fixtures and an ASP.NET host with EF InMemory. SQL Server, Docker and external delivery providers remain unverified.
- Static demo actions persist only in that browser session, per demo role. They are synthetic and never touch a real clinic database.
- All earlier dependency, SQL, SMTP, development credentials and production-audit caveats remain.
