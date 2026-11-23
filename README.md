# CarePoint Clinic API

.NET 8 clinic management API. This repository retains its existing APIs/Core/Service/Repository architecture and SQL Server data model.

## Run locally

Prerequisites: .NET 8 SDK and SQL Server 2019+ (LocalDB is supported on Windows).

```sh
dotnet restore ClinicBack.sln
dotnet run --project TourSite.APIs --launch-profile http
```

The development connection defaults to Windows LocalDB. On Linux/macOS, point `ConnectionStrings__Default` to an accessible SQL Server instance before running. The API listens on `http://localhost:5295`; Swagger is at `http://localhost:5295/swagger`.

Keep `Clinic-Front-New` beside this folder and start its Angular development server on port 4200. Its proxy routes `/api` and `/images` to port 5295, so authentication stays same-origin.

## Docker local stack

Keep the two repository folders side by side:

```text
workspace/
  ClinicBack-New/
  Clinic-Front-New/
```

From this folder, copy `.env.example` to `.env`, replace all three placeholder values, then:

```sh
docker compose up --build
```

Open `http://localhost:4200`. SQL Server persists in the `clinic-data` volume. `docker compose down` preserves data; adding `-v` deletes it. SQL Server containers require a supported x86-64 host or a separately managed SQL Server. Docker configuration is supplied but was not executed in the delivery environment.

This Compose stack is **development-only**. Its API environment is Development, HTTP cookies are allowed, and database seeding creates demo accounts. Do not expose it to the Internet.

## Configuration

- `ConnectionStrings:Default`: SQL Server connection string.
- `JwtSettings:Key`: long random signing key, supplied outside source control in production.
- `JwtSettings:Issuer`, `Audience`, `AccessTokenMinutes`, `RefreshTokenDays`: token settings.
- `FrontendUrl`: frontend origin used for email-confirmation/password-reset links.
- `BaseUrl`: existing asset-origin setting, if used by deployment.
- `EmailSettings:*`: SMTP provider configuration. Recovery email needs working SMTP.
- `Seed:*`: development account settings. See the existing seeder before enabling it against any real database.

Use environment variables (`__` replaces `:`) or .NET user-secrets. Never commit real credentials or patient data. Demo credentials already exist in the original development configuration; replace them before using any real data.

## Tests

```sh
dotnet test Clinic.IntegrationTests/Clinic.IntegrationTests.csproj
```

Seven tests run the real ASP.NET API host with isolated EF InMemory data. They cover five staff logins/role authorization, patient registration/session refresh/logout, catalog CRUD, status serialization, and patient denial on medication writes. They do not validate SQL Server migrations, constraints, transaction isolation or production SMTP.

## Changes and documentation

- [Backend changes and caveats](docs/BACKEND-CHANGES.md)
- Frontend repository: `docs/ARCHITECTURE.md`, `docs/FEATURES.md`, `docs/TESTING.md`, `docs/LOCAL-RUN.md`.

Do not use this project for real clinical data until deployment security, backups, audit logging, data-retention rules and local healthcare/privacy obligations have been reviewed.
