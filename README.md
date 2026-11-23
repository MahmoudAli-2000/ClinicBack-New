# Clinic Back

This is the .NET backend for the clinic management system.

## Local development

From the repository root:

```
dotnet restore
cd TourSite.APIs
dotnet run --launch-profile http
```

## Default connection string

```json
"Default": "Server=(localdb)\\mssqllocaldb;Database=ClinicDb.Development;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True;"
```

## Seeded admin credentials

- Email: `admin@clinic.local`
- Password: `Admin@123`

## Role model

The application includes roles for Admin, Manager, Receptionist, Nurse, Doctor, and Patient.

The API enforces authorization on protected endpoints and uses cookie-based JWT authentication for the frontend.