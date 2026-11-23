using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourSite.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddImageCover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageCover",
                table: "StaffMembers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageCover",
                table: "Specialties",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageCover",
                table: "Patients",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageCover",
                table: "Medications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageCover",
                table: "Doctors",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageCover",
                table: "Clinics",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageCover",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageCover",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "ImageCover",
                table: "Specialties");

            migrationBuilder.DropColumn(
                name: "ImageCover",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ImageCover",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "ImageCover",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "ImageCover",
                table: "Clinics");

            migrationBuilder.DropColumn(
                name: "ImageCover",
                table: "AspNetUsers");
        }
    }
}
