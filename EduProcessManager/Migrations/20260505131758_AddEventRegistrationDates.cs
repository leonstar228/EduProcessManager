using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduProcessManager.Migrations
{
    /// <inheritdoc />
    public partial class AddEventRegistrationDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AvailableFrom",
                table: "Tests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AvailableUntil",
                table: "Tests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationAvailableFrom",
                table: "EduEvents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationAvailableUntil",
                table: "EduEvents",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableFrom",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "AvailableUntil",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "RegistrationAvailableFrom",
                table: "EduEvents");

            migrationBuilder.DropColumn(
                name: "RegistrationAvailableUntil",
                table: "EduEvents");
        }
    }
}
