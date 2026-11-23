# Arabic / English, database setup and profile photos

This update continues the existing Angular 17 and .NET 8 projects. It does not replace their architecture. Angular components keep separate TypeScript, HTML and CSS files.

## What changed

### Frontend

- Transloco 7, bundled `src/assets/i18n/en.json` and `ar.json`, instant switching without a new build or page reload.
- `LanguageService` controls document `lang` and `dir`. Angular CDK `BidiModule` supplies the direction context. Layout styles use logical spacing and positioning; the mobile navigation slides from the correct edge.
- `UiPipe` maps English UI source phrases to stable JSON keys. To add a phrase, add matching keys in both JSON files and use `{{ 'Your phrase' | ui }}`. English is the fallback. User-entered records are not machine-translated.
- `ClinicDatePipe` chooses Angular's registered Arabic or English locale at runtime. Server timestamps stay UTC. Browser-local appointment times and documented UTC scheduling rules are unchanged.
- A language selector appears on public authentication screens and in the authenticated header. Guests have a separate local preference. Signed-in choices are cached under the account ID and saved through `/api/Profile/language`; signing out does not copy one account's choice to another.
- Catalog edit forms accept Arabic names. Existing English fields stay intact. Catalog displays prefer the Arabic column when present, otherwise English. Lookup lists use the server's `displayName` where supplied.
- Account settings include a photo upload. The header shows the signed-in user's name and photo, including on mobile. Empty or failed images fall back to initials. Profile edits update the header without signing in again.

### Backend language negotiation

`RequestLocalization` supports `en` and `ar`, with neutral English as the default. Precedence:

1. `?culture=ar` (or `?culture=en`), optional `ui-culture` follows ASP.NET's standard provider behavior.
2. `Accept-Language: ar-EG,ar;q=0.9,en;q=0.8`, matched to a supported parent culture.
3. English.

The frontend sends `Accept-Language` on API requests, including login. Language preference does not silently override explicit request headers from API clients.

```sh
curl -H 'Accept-Language: ar' http://localhost:5295/api/Specialties/1
curl 'http://localhost:5295/api/Specialties/1?culture=en'
```

Resources live in `TourSite.Core/Localization/Messages.resx` and `Messages.ar.resx`. They cover fixed domain errors, controller response messages, authentication/authorization envelopes and common Identity password errors. Arabic model-validation responses identify the invalid field using its stable contract name. Dynamic exception messages without a matching resource use a translated status-level message instead of leaking English internals. Logs remain technical, in English. Server errors no longer expose stack traces or SQL details to callers. Response cache keys include culture to avoid mixing languages.

Medical notes, names entered by staff, review text, existing notification bodies and provider-delivered email content are not automatically translated. Those are authored data, not UI labels. The existing email templates are outside this UI/API-resource translation pass. API enum values, IDs, field names, role values, drug strengths and clinical codes remain stable across languages. Do not translate protocol values sent in a request.

## The actual migration

Included migration: `20261123211354_BilingualCatalog` in `TourSite.Repository/Migrations`, with designer and updated model snapshot.

It adds nullable SQL Server `nvarchar` columns:

| Table | New columns |
|---|---|
| Specialties | SpecialtyNameAr (120), DescriptionAr (500) |
| Clinics | ClinicNameAr (150), WorkingHoursNotesAr (500) |
| Medications | GenericNameAr (200), BrandNameAr (200), DosageFormAr (100) |

Existing English values and primary keys are preserved. The Arabic DTO fields are writable on catalog create/update routes and returned alongside English. Specialty, clinic and medication DTOs add a culture-aware `displayName`. Doctor DTOs also expose `specialtyNameAr`. Arabic specialty/clinic/medication-name searches are supported. Person names and clinical text are not duplicated into translation columns.

Language is stored as an Identity claim (`carepoint:language`). Photos reuse `AspNetUsers.ImageCover`. Neither needs an extra schema column.

## First local run, command by command

Prerequisites: .NET SDK 8, Node compatible with Angular 17 (Node 20 LTS recommended), npm, and reachable SQL Server. EF tool version must match this project's EF Core 9.0.2 packages, even though the target runtime is .NET 8.

Keep the two repository directories side by side.

### Option A: Docker development setup

```sh
cd ClinicBack-New
cp .env.example .env
# Edit .env and replace every required development secret.
docker compose up --build
```

The database health check runs before the API starts. Development API startup applies pending migrations, then seeds. Open `http://localhost:4200`. The API is on `http://localhost:5295`; Swagger is `/swagger`. The `clinic-data` volume persists SQL data; `clinic-uploads` persists profile pictures. Docker/SQL execution still needs validation on your machine. Do not expose this development stack to the internet.

### Option B: .NET CLI and local SQL Server, Bash

```sh
cd ClinicBack-New
dotnet restore
dotnet tool restore
# If starting from an archive without the local tool manifest:
# dotnet new tool-manifest
# dotnet tool install dotnet-ef --version 9.0.2

# Set the same real SQL Server connection for runtime and design-time EF.
export CLINIC_DB='Server=localhost,1433;Database=ClinicDb;User Id=sa;Password=YOUR_LOCAL_PASSWORD;TrustServerCertificate=True'
export ConnectionStrings__Default="$CLINIC_DB"
export ASPNETCORE_ENVIRONMENT=Development
export Seed__Enabled=true
# Set local-only credentials. Never reuse the example passwords in a real clinic.
export Seed__AdminEmail=admin@clinic.local
export Seed__AdminPassword='CHANGE_TO_A_LOCAL_TEST_PASSWORD'
export Seed__StaffPassword='CHANGE_TO_A_LOCAL_TEST_PASSWORD'
export Seed__PatientPassword='CHANGE_TO_A_LOCAL_TEST_PASSWORD'

dotnet ef migrations list --project TourSite.Repository --startup-project TourSite.APIs
dotnet ef database update --project TourSite.Repository --startup-project TourSite.APIs
dotnet run --project TourSite.APIs
```

In another terminal:

```sh
cd Clinic-Front-New
npm ci --legacy-peer-deps
npm start
```

Use `http://localhost:4200`. `proxy.conf.json` forwards `/api` and `/images` to port 5295.

### Option C: PowerShell and Visual Studio

```powershell
cd ClinicBack-New
dotnet restore
dotnet tool restore
$env:CLINIC_DB = 'Server=(localdb)\mssqllocaldb;Database=ClinicDb.Development;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True'
$env:ConnectionStrings__Default = $env:CLINIC_DB
$env:ASPNETCORE_ENVIRONMENT = 'Development'
$env:Seed__Enabled = 'true'
dotnet ef database update --project TourSite.Repository --startup-project TourSite.APIs
dotnet run --project TourSite.APIs
```

Equivalent Visual Studio Package Manager Console command, with the API selected as startup project:

```powershell
Update-Database -Project TourSite.Repository -StartupProject TourSite.APIs
```

Do **not** run `Add-Migration BilingualCatalog` again. It is already included. For your next deliberate model change:

```sh
dotnet ef migrations add YourNextChange --project TourSite.Repository --startup-project TourSite.APIs
dotnet ef database update --project TourSite.Repository --startup-project TourSite.APIs
```

### Startup behavior and seed

Every relational startup applies pending migrations, using EF's migration history to avoid repeating completed migrations. Startup now stops if migration/seed fails instead of logging the error and serving a broken database. Use one deployment migration runner in a multi-instance production environment rather than racing multiple API replicas at startup.

The included Development config enables seed; the base config disables it. An explicit `Seed:Enabled` overrides the environment fallback. Disable it in Production (`Seed__Enabled=false`). Production does not seed unless explicitly enabled. Seed creates the known demo roles/accounts and example clinics, specialties, medications, patients, appointments and clinical data from the existing seed. The bilingual pass fills only missing Arabic values for recognized demo catalog entries; it can be run repeatedly without adding duplicate entries. It does not overwrite custom Arabic content or guess translations for arbitrary existing clinic data.

The admin email is configured by `Seed:AdminEmail`. Staff examples include `manager@clinic.local`, `frontdesk@clinic.local`, `nurse@clinic.local` and `ahmed.doctor@clinic.local`. For development only, unchanged existing defaults are Admin@123 for the admin and Password@123 for seeded staff/patients. Override those settings **before first seed**. Seed does not reset existing user passwords when a setting changes. Use normal account recovery/administration for existing users.

The checked-in `docs/MIGRATIONS.sql` is an EF-generated idempotent SQL Server schema script. Applying it does **not** create demo user/data records, since those use application code and Identity. Start the development API with seed enabled to populate them. Back up your database and review the SQL script before applying it to an existing database. A rollback removes Arabic columns and loses their values, so do not roll back casually.

## Profile photo API and storage

- `POST /api/Profile/photo`, authenticated, multipart field name `image`.
- Own account only. No user ID from the browser is accepted.
- JPEG/PNG/WebP, at most 2 MB; validates the decoded format, dimensions and pixel count.
- Auto-orients, crops to 512×512, strips metadata and encodes WebP with a generated filename.
- Saves the relative path to the existing `User.ImageCover` field. The returned current-user DTO includes a root-relative URL and full name.
- A replaced generated profile image is removed from the profile folder. Uploaded filenames cannot select a filesystem path.
- Static profile images are visible to anyone who knows their URL. Do not use this location for confidential medical documents.
- Back up uploads separately from SQL. Use a persistent volume or durable object storage in a real deployment.
- Existing ImageSharp dependency advisories still need the production dependency/licensing decision described in DEPENDENCY-AUDIT. Upload validation does not remove those advisories.

## Verification and limitations

- Angular production build and backend Release build passed. All 33 browser checks and all 11 API-host tests passed.
- Browser tests use fixtures, not a real SQL clinic database. API tests run a real ASP.NET host using EF InMemory.
- Added tests cover locale negotiation/query override, Arabic validation/domain errors, language account isolation, bilingual seed idempotence and valid/invalid authenticated photo uploads.
- Arabic responsive browser checks cover 320, 390, 768 and 1440 pixels, with desktop/mobile visual inspection.
- EF generated the migration and idempotent SQL successfully. SQL Server migration application, Docker, SMTP and production deployment are **not** verified by those tests.
- Frontend production audit currently reports 22 findings (2 critical, 11 high, 9 moderate). Existing backend ImageSharp advisories remain. This is an implementation delivery, not a claim that the system is production-audited.

## Static demo

The demo build is separate from the real app. `tests/demo-preview.js` intercepts API calls using synthetic data. It supports role selection, per-role language/theme settings and a locally simulated profile photo. It never creates real clinic records or calls the live backend. Never put it into a production deployment. Serve its files at the origin root with SPA fallback. The visible UI DEMO label must remain.
