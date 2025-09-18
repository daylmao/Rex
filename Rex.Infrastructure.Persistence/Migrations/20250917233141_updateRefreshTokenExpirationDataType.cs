using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rex.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateRefreshTokenExpirationDataType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Expiration",
                table: "RefreshToken",
                type: "timestamptz",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Expiration",
                table: "RefreshToken",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamptz");
        }
    }
}
