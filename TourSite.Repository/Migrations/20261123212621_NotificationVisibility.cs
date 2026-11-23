using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourSite.Repository.Migrations
{
    /// <inheritdoc />
    public partial class NotificationVisibility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "HiddenAtUtc",
                table: "Notifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AppointmentUpdates",
                table: "NotificationPreferences",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HiddenAtUtc",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "AppointmentUpdates",
                table: "NotificationPreferences");
        }
    }
}
