using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourSite.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicCode",
                table: "StaffMembers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PublicCode",
                table: "Patients",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PublicCode",
                table: "Doctors",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
UPDATE Patients
SET PublicCode = CONCAT('PH-', FORMAT(YEAR(CreatedAtUtc), '0000'), '-', RIGHT('000000' + CAST(Id AS VARCHAR(10)), 6))
WHERE PublicCode = '';");

            migrationBuilder.Sql(@"
UPDATE Doctors
SET PublicCode = CONCAT('DC-', FORMAT(YEAR(CreatedAtUtc), '0000'), '-', RIGHT('000000' + CAST(Id AS VARCHAR(10)), 6))
WHERE PublicCode = '';");

            migrationBuilder.Sql(@"
UPDATE s
SET s.PublicCode = CONCAT(
    CASE WHEN EXISTS (
        SELECT 1 FROM AspNetUserRoles ur
        JOIN AspNetRoles r ON r.Id = ur.RoleId
        WHERE ur.UserId = s.UserId AND r.Name IN ('Admin', 'Manager', 'Receptionist'))
    THEN 'AD' ELSE 'ST' END,
    '-', FORMAT(YEAR(s.CreatedAtUtc), '0000'), '-', RIGHT('000000' + CAST(s.Id AS VARCHAR(10)), 6))
FROM StaffMembers s
WHERE s.PublicCode = '';");

            migrationBuilder.CreateIndex(
                name: "UX_StaffMembers_PublicCode",
                table: "StaffMembers",
                column: "PublicCode",
                unique: true,
                filter: "[PublicCode] <> ''");

            migrationBuilder.CreateIndex(
                name: "UX_Patients_PublicCode",
                table: "Patients",
                column: "PublicCode",
                unique: true,
                filter: "[PublicCode] <> ''");

            migrationBuilder.CreateIndex(
                name: "UX_Doctors_PublicCode",
                table: "Doctors",
                column: "PublicCode",
                unique: true,
                filter: "[PublicCode] <> ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_StaffMembers_PublicCode",
                table: "StaffMembers");

            migrationBuilder.DropIndex(
                name: "UX_Patients_PublicCode",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "UX_Doctors_PublicCode",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "PublicCode",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "PublicCode",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PublicCode",
                table: "Doctors");
        }
    }
}
