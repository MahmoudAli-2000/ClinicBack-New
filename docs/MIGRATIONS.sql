IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [FullName] nvarchar(150) NOT NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [Clinics] (
        [Id] int NOT NULL IDENTITY,
        [ClinicCode] nvarchar(30) NOT NULL,
        [ClinicName] nvarchar(150) NOT NULL,
        [Phone] nvarchar(30) NULL,
        [Email] nvarchar(254) NULL,
        [AddressLine1] nvarchar(250) NULL,
        [AddressLine2] nvarchar(250) NULL,
        [City] nvarchar(100) NULL,
        [Country] nvarchar(100) NULL,
        [WorkingHoursNotes] nvarchar(500) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [UpdatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_Clinics] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [Medications] (
        [Id] int NOT NULL IDENTITY,
        [MedicationCode] nvarchar(50) NULL,
        [BrandName] nvarchar(200) NULL,
        [GenericName] nvarchar(200) NOT NULL,
        [DosageForm] nvarchar(100) NULL,
        [Strength] nvarchar(100) NULL,
        [Unit] nvarchar(50) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_Medications] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [Specialties] (
        [Id] int NOT NULL IDENTITY,
        [SpecialtyName] nvarchar(120) NOT NULL,
        [Description] nvarchar(500) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_Specialties] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [NotificationPreferences] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [AllowInApp] bit NOT NULL,
        [AllowEmail] bit NOT NULL,
        [AllowSMS] bit NOT NULL,
        [AllowPush] bit NOT NULL,
        [AllowWhatsApp] bit NOT NULL,
        [AppointmentReminders] bit NOT NULL,
        [GeneralUpdates] bit NOT NULL,
        [UpdatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_NotificationPreferences] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_NotificationPreferences_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [Patients] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NULL,
        [PatientNo] nvarchar(30) NOT NULL,
        [FirstName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [DateOfBirth] datetime2 NULL,
        [Gender] nvarchar(20) NULL,
        [Phone] nvarchar(30) NULL,
        [Email] nvarchar(254) NULL,
        [AddressLine1] nvarchar(250) NULL,
        [AddressLine2] nvarchar(250) NULL,
        [City] nvarchar(100) NULL,
        [EmergencyContactName] nvarchar(150) NULL,
        [EmergencyContactPhone] nvarchar(30) NULL,
        [Status] nvarchar(20) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [UpdatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_Patients] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Patients_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE SET NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [RefreshTokens] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [TokenHash] nvarchar(128) NOT NULL,
        [ExpiresAtUtc] datetime2 NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [RevokedAtUtc] datetime2 NULL,
        [ReplacedByTokenHash] nvarchar(128) NULL,
        [CreatedByIp] nvarchar(200) NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RefreshTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [StaffMembers] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NULL,
        [EmployeeNo] nvarchar(30) NOT NULL,
        [FirstName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [Phone] nvarchar(30) NULL,
        [Email] nvarchar(254) NULL,
        [PrimaryClinicId] int NULL,
        [JobTitle] nvarchar(120) NULL,
        [Status] nvarchar(20) NOT NULL,
        [HireDate] datetime2 NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [UpdatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_StaffMembers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StaffMembers_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_StaffMembers_Clinics_PrimaryClinicId] FOREIGN KEY ([PrimaryClinicId]) REFERENCES [Clinics] ([Id]) ON DELETE SET NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [Doctors] (
        [Id] int NOT NULL IDENTITY,
        [StaffId] int NOT NULL,
        [SpecialtyId] int NOT NULL,
        [LicenseNo] nvarchar(80) NOT NULL,
        [ConsultationDurationMinutes] smallint NOT NULL,
        [DefaultConsultationFee] decimal(12,2) NULL,
        [Bio] nvarchar(1500) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [UpdatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_Doctors] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Doctors_Specialties_SpecialtyId] FOREIGN KEY ([SpecialtyId]) REFERENCES [Specialties] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Doctors_StaffMembers_StaffId] FOREIGN KEY ([StaffId]) REFERENCES [StaffMembers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [DoctorClinics] (
        [Id] int NOT NULL IDENTITY,
        [DoctorId] int NOT NULL,
        [ClinicId] int NOT NULL,
        [IsPrimaryClinic] bit NOT NULL,
        [ConsultationFee] decimal(12,2) NULL,
        [IsActive] bit NOT NULL,
        [JoinedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_DoctorClinics] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DoctorClinics_Clinics_ClinicId] FOREIGN KEY ([ClinicId]) REFERENCES [Clinics] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DoctorClinics_Doctors_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [Doctors] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [DoctorTimeOffs] (
        [Id] int NOT NULL IDENTITY,
        [DoctorId] int NOT NULL,
        [ClinicId] int NULL,
        [StartAtUtc] datetime2 NOT NULL,
        [EndAtUtc] datetime2 NOT NULL,
        [Reason] nvarchar(500) NULL,
        [Status] nvarchar(20) NOT NULL,
        [CreatedByUserId] nvarchar(450) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_DoctorTimeOffs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DoctorTimeOffs_AspNetUsers_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_DoctorTimeOffs_Clinics_ClinicId] FOREIGN KEY ([ClinicId]) REFERENCES [Clinics] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_DoctorTimeOffs_Doctors_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [Doctors] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [Appointments] (
        [Id] int NOT NULL IDENTITY,
        [PatientId] int NOT NULL,
        [DoctorClinicId] int NOT NULL,
        [ScheduledStartAtUtc] datetime2 NOT NULL,
        [ScheduledEndAtUtc] datetime2 NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        [BookingSource] nvarchar(20) NOT NULL,
        [Reason] nvarchar(1000) NULL,
        [Notes] nvarchar(1500) NULL,
        [CreatedByUserId] nvarchar(450) NULL,
        [CancelledAtUtc] datetime2 NULL,
        [ReminderSentAtUtc] datetime2 NULL,
        [CancellationReason] nvarchar(500) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [UpdatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Appointments_AspNetUsers_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Appointments_DoctorClinics_DoctorClinicId] FOREIGN KEY ([DoctorClinicId]) REFERENCES [DoctorClinics] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Appointments_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [DoctorSchedules] (
        [Id] int NOT NULL IDENTITY,
        [DoctorClinicId] int NOT NULL,
        [DayOfWeek] tinyint NOT NULL,
        [StartTime] time NOT NULL,
        [EndTime] time NOT NULL,
        [SlotDurationMinutes] smallint NOT NULL,
        [EffectiveFrom] datetime2 NULL,
        [EffectiveTo] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_DoctorSchedules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DoctorSchedules_DoctorClinics_DoctorClinicId] FOREIGN KEY ([DoctorClinicId]) REFERENCES [DoctorClinics] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [Notifications] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [AppointmentId] int NULL,
        [NotificationType] nvarchar(50) NOT NULL,
        [Channel] nvarchar(20) NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Message] nvarchar(2000) NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        [ScheduledAtUtc] datetime2 NULL,
        [SentAtUtc] datetime2 NULL,
        [ReadAtUtc] datetime2 NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Notifications_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Notifications_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [Reviews] (
        [Id] int NOT NULL IDENTITY,
        [AppointmentId] int NOT NULL,
        [DoctorRating] tinyint NULL,
        [ClinicRating] tinyint NULL,
        [StaffRating] tinyint NULL,
        [Comment] nvarchar(2000) NULL,
        [IsAnonymous] bit NOT NULL,
        [ReviewedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_Reviews] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Reviews_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [Visits] (
        [Id] int NOT NULL IDENTITY,
        [AppointmentId] int NOT NULL,
        [CheckInAtUtc] datetime2 NULL,
        [VisitStartAtUtc] datetime2 NULL,
        [VisitEndAtUtc] datetime2 NULL,
        [ChiefComplaint] nvarchar(1500) NULL,
        [DoctorNotes] nvarchar(max) NULL,
        [Status] nvarchar(20) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [UpdatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_Visits] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Visits_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [Diagnoses] (
        [Id] int NOT NULL IDENTITY,
        [VisitId] int NOT NULL,
        [DiagnosisCode] nvarchar(30) NULL,
        [DiagnosisName] nvarchar(250) NOT NULL,
        [DiagnosisType] nvarchar(20) NOT NULL,
        [Notes] nvarchar(1500) NULL,
        [DiagnosedByDoctorId] int NULL,
        [DiagnosedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_Diagnoses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Diagnoses_Doctors_DiagnosedByDoctorId] FOREIGN KEY ([DiagnosedByDoctorId]) REFERENCES [Doctors] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Diagnoses_Visits_VisitId] FOREIGN KEY ([VisitId]) REFERENCES [Visits] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [Prescriptions] (
        [Id] int NOT NULL IDENTITY,
        [VisitId] int NOT NULL,
        [PrescribedByDoctorId] int NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        [IssuedAtUtc] datetime2 NOT NULL,
        [Notes] nvarchar(1500) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [UpdatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_Prescriptions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Prescriptions_Doctors_PrescribedByDoctorId] FOREIGN KEY ([PrescribedByDoctorId]) REFERENCES [Doctors] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Prescriptions_Visits_VisitId] FOREIGN KEY ([VisitId]) REFERENCES [Visits] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [VitalSigns] (
        [Id] int NOT NULL IDENTITY,
        [VisitId] int NOT NULL,
        [WeightKg] decimal(6,2) NULL,
        [HeightCm] decimal(6,2) NULL,
        [SystolicBP] smallint NULL,
        [DiastolicBP] smallint NULL,
        [HeartRateBpm] smallint NULL,
        [TemperatureC] decimal(4,1) NULL,
        [OxygenSaturationPct] decimal(5,2) NULL,
        [RespiratoryRate] smallint NULL,
        [RecordedByStaffId] int NULL,
        [RecordedAtUtc] datetime2 NOT NULL,
        [Notes] nvarchar(500) NULL,
        CONSTRAINT [PK_VitalSigns] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_VitalSigns_StaffMembers_RecordedByStaffId] FOREIGN KEY ([RecordedByStaffId]) REFERENCES [StaffMembers] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_VitalSigns_Visits_VisitId] FOREIGN KEY ([VisitId]) REFERENCES [Visits] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE TABLE [PrescriptionItems] (
        [Id] int NOT NULL IDENTITY,
        [PrescriptionId] int NOT NULL,
        [MedicationId] int NOT NULL,
        [Dose] nvarchar(100) NULL,
        [Frequency] nvarchar(150) NULL,
        [DurationDays] smallint NULL,
        [Route] nvarchar(100) NULL,
        [Quantity] decimal(10,2) NULL,
        [Instructions] nvarchar(1000) NULL,
        CONSTRAINT [PK_PrescriptionItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PrescriptionItems_Medications_MedicationId] FOREIGN KEY ([MedicationId]) REFERENCES [Medications] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PrescriptionItems_Prescriptions_PrescriptionId] FOREIGN KEY ([PrescriptionId]) REFERENCES [Prescriptions] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_Appointments_CreatedByUserId] ON [Appointments] ([CreatedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_Appointments_DoctorClinicId] ON [Appointments] ([DoctorClinicId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_Appointments_PatientId] ON [Appointments] ([PatientId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_Appointments_PatientId_ScheduledStartAtUtc] ON [Appointments] ([PatientId], [ScheduledStartAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_Appointments_ScheduledStartAtUtc] ON [Appointments] ([ScheduledStartAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_Appointments_Status] ON [Appointments] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UX_Appointments_DoctorClinic_Start_Active] ON [Appointments] ([DoctorClinicId], [ScheduledStartAtUtc]) WHERE [Status] IN (''Scheduled'',''Confirmed'',''CheckedIn'',''InProgress'')');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Clinics_ClinicCode] ON [Clinics] ([ClinicCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_Clinics_ClinicName] ON [Clinics] ([ClinicName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_Diagnoses_DiagnosedByDoctorId] ON [Diagnoses] ([DiagnosedByDoctorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_Diagnoses_VisitId] ON [Diagnoses] ([VisitId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_DoctorClinics_ClinicId] ON [DoctorClinics] ([ClinicId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DoctorClinics_DoctorId_ClinicId] ON [DoctorClinics] ([DoctorId], [ClinicId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Doctors_LicenseNo] ON [Doctors] ([LicenseNo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_Doctors_SpecialtyId] ON [Doctors] ([SpecialtyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Doctors_StaffId] ON [Doctors] ([StaffId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_DoctorSchedules_DoctorClinicId_DayOfWeek] ON [DoctorSchedules] ([DoctorClinicId], [DayOfWeek]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_DoctorTimeOffs_ClinicId] ON [DoctorTimeOffs] ([ClinicId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_DoctorTimeOffs_CreatedByUserId] ON [DoctorTimeOffs] ([CreatedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_DoctorTimeOffs_DoctorId_StartAtUtc_EndAtUtc] ON [DoctorTimeOffs] ([DoctorId], [StartAtUtc], [EndAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_Medications_GenericName] ON [Medications] ([GenericName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Medications_MedicationCode] ON [Medications] ([MedicationCode]) WHERE [MedicationCode] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE UNIQUE INDEX [IX_NotificationPreferences_UserId] ON [NotificationPreferences] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_Notifications_AppointmentId] ON [Notifications] ([AppointmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_Notifications_ScheduledAtUtc] ON [Notifications] ([ScheduledAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_Notifications_UserId_Status_CreatedAtUtc] ON [Notifications] ([UserId], [Status], [CreatedAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_Patients_Email] ON [Patients] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_Patients_LastName_FirstName] ON [Patients] ([LastName], [FirstName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Patients_PatientNo] ON [Patients] ([PatientNo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_Patients_Phone] ON [Patients] ([Phone]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Patients_UserId] ON [Patients] ([UserId]) WHERE [UserId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_PrescriptionItems_MedicationId] ON [PrescriptionItems] ([MedicationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_PrescriptionItems_PrescriptionId] ON [PrescriptionItems] ([PrescriptionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_Prescriptions_PrescribedByDoctorId] ON [Prescriptions] ([PrescribedByDoctorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_Prescriptions_VisitId] ON [Prescriptions] ([VisitId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RefreshTokens_TokenHash] ON [RefreshTokens] ([TokenHash]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_UserId_ExpiresAtUtc] ON [RefreshTokens] ([UserId], [ExpiresAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Reviews_AppointmentId] ON [Reviews] ([AppointmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_Reviews_ReviewedAtUtc] ON [Reviews] ([ReviewedAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Specialties_SpecialtyName] ON [Specialties] ([SpecialtyName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StaffMembers_EmployeeNo] ON [StaffMembers] ([EmployeeNo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_StaffMembers_PrimaryClinicId] ON [StaffMembers] ([PrimaryClinicId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_StaffMembers_UserId] ON [StaffMembers] ([UserId]) WHERE [UserId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Visits_AppointmentId] ON [Visits] ([AppointmentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_VitalSigns_RecordedByStaffId] ON [VitalSigns] ([RecordedByStaffId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    CREATE INDEX [IX_VitalSigns_VisitId] ON [VitalSigns] ([VisitId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011102243_InitialClinic'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261011102243_InitialClinic', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019143348_AddImageCover'
)
BEGIN
    ALTER TABLE [StaffMembers] ADD [ImageCover] nvarchar(500) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019143348_AddImageCover'
)
BEGIN
    ALTER TABLE [Specialties] ADD [ImageCover] nvarchar(500) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019143348_AddImageCover'
)
BEGIN
    ALTER TABLE [Patients] ADD [ImageCover] nvarchar(500) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019143348_AddImageCover'
)
BEGIN
    ALTER TABLE [Medications] ADD [ImageCover] nvarchar(500) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019143348_AddImageCover'
)
BEGIN
    ALTER TABLE [Doctors] ADD [ImageCover] nvarchar(500) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019143348_AddImageCover'
)
BEGIN
    ALTER TABLE [Clinics] ADD [ImageCover] nvarchar(500) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019143348_AddImageCover'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [ImageCover] nvarchar(500) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019143348_AddImageCover'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261019143348_AddImageCover', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019150540_AddPublicCode'
)
BEGIN
    ALTER TABLE [StaffMembers] ADD [PublicCode] nvarchar(20) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019150540_AddPublicCode'
)
BEGIN
    ALTER TABLE [Patients] ADD [PublicCode] nvarchar(20) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019150540_AddPublicCode'
)
BEGIN
    ALTER TABLE [Doctors] ADD [PublicCode] nvarchar(20) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019150540_AddPublicCode'
)
BEGIN

    UPDATE Patients
    SET PublicCode = CONCAT('PH-', FORMAT(YEAR(CreatedAtUtc), '0000'), '-', RIGHT('000000' + CAST(Id AS VARCHAR(10)), 6))
    WHERE PublicCode = '';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019150540_AddPublicCode'
)
BEGIN

    UPDATE Doctors
    SET PublicCode = CONCAT('DC-', FORMAT(YEAR(CreatedAtUtc), '0000'), '-', RIGHT('000000' + CAST(Id AS VARCHAR(10)), 6))
    WHERE PublicCode = '';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019150540_AddPublicCode'
)
BEGIN

    UPDATE s
    SET s.PublicCode = CONCAT(
        CASE WHEN EXISTS (
            SELECT 1 FROM AspNetUserRoles ur
            JOIN AspNetRoles r ON r.Id = ur.RoleId
            WHERE ur.UserId = s.UserId AND r.Name IN ('Admin', 'Manager', 'Receptionist'))
        THEN 'AD' ELSE 'ST' END,
        '-', FORMAT(YEAR(s.CreatedAtUtc), '0000'), '-', RIGHT('000000' + CAST(s.Id AS VARCHAR(10)), 6))
    FROM StaffMembers s
    WHERE s.PublicCode = '';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019150540_AddPublicCode'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UX_StaffMembers_PublicCode] ON [StaffMembers] ([PublicCode]) WHERE [PublicCode] <> ''''');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019150540_AddPublicCode'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UX_Patients_PublicCode] ON [Patients] ([PublicCode]) WHERE [PublicCode] <> ''''');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019150540_AddPublicCode'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UX_Doctors_PublicCode] ON [Doctors] ([PublicCode]) WHERE [PublicCode] <> ''''');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261019150540_AddPublicCode'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261019150540_AddPublicCode', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123211354_BilingualCatalog'
)
BEGIN
    ALTER TABLE [Specialties] ADD [DescriptionAr] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123211354_BilingualCatalog'
)
BEGIN
    ALTER TABLE [Specialties] ADD [SpecialtyNameAr] nvarchar(120) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123211354_BilingualCatalog'
)
BEGIN
    ALTER TABLE [Medications] ADD [BrandNameAr] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123211354_BilingualCatalog'
)
BEGIN
    ALTER TABLE [Medications] ADD [DosageFormAr] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123211354_BilingualCatalog'
)
BEGIN
    ALTER TABLE [Medications] ADD [GenericNameAr] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123211354_BilingualCatalog'
)
BEGIN
    ALTER TABLE [Clinics] ADD [ClinicNameAr] nvarchar(150) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123211354_BilingualCatalog'
)
BEGIN
    ALTER TABLE [Clinics] ADD [WorkingHoursNotesAr] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261123211354_BilingualCatalog'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261123211354_BilingualCatalog', N'9.0.2');
END;

COMMIT;
GO

