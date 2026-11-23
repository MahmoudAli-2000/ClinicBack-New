using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourSite.Repository.Migrations
{
    /// <inheritdoc />
    public partial class BilingualCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "Specialties",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecialtyNameAr",
                table: "Specialties",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BrandNameAr",
                table: "Medications",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DosageFormAr",
                table: "Medications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GenericNameAr",
                table: "Medications",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClinicNameAr",
                table: "Clinics",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkingHoursNotesAr",
                table: "Clinics",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "Specialties");

            migrationBuilder.DropColumn(
                name: "SpecialtyNameAr",
                table: "Specialties");

            migrationBuilder.DropColumn(
                name: "BrandNameAr",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "DosageFormAr",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "GenericNameAr",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "ClinicNameAr",
                table: "Clinics");

            migrationBuilder.DropColumn(
                name: "WorkingHoursNotesAr",
                table: "Clinics");
        }
    }
}
