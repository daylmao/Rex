using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rex.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdjustProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "RefreshCode",
                table: "Code");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "RefreshToken",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "RefreshToken",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "RefreshToken",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "RefreshToken",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RefreshToken");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "RefreshToken",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "RefreshCode",
                table: "Code",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
