using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rex.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeBirthdayDataType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserRole",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "UserRole",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "UserRole",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserRole",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Birthday",
                table: "User",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamptz");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserRole");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Birthday",
                table: "User",
                type: "timestamptz",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");
        }
    }
}
