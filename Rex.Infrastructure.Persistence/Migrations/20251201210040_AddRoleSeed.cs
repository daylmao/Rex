using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Rex.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GroupRole",
                columns: new[] { "PkGroupRoleId", "CreatedAt", "Deleted", "DeletedAt", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("31097fd3-a3c2-409d-9730-8dabffcd04a4"), new DateTime(2025, 12, 1, 21, 0, 39, 453, DateTimeKind.Utc).AddTicks(125), false, null, "Member", null },
                    { new Guid("9ca44144-2f55-4705-8753-85b8b86b7489"), new DateTime(2025, 12, 1, 21, 0, 39, 453, DateTimeKind.Utc).AddTicks(119), false, null, "Leader", null },
                    { new Guid("a97a1660-afb4-4457-a4e1-bb4164afe397"), new DateTime(2025, 12, 1, 21, 0, 39, 453, DateTimeKind.Utc).AddTicks(121), false, null, "Mentor", null },
                    { new Guid("b7904eff-7008-4bb7-aea2-44fb27b688ff"), new DateTime(2025, 12, 1, 21, 0, 39, 453, DateTimeKind.Utc).AddTicks(123), false, null, "Moderator", null }
                });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "PkUserRoleId", "CreatedAt", "Deleted", "DeletedAt", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a96774e4-170c-4a2a-b87e-d4e3be0d4443"), new DateTime(2025, 12, 1, 21, 0, 39, 452, DateTimeKind.Utc).AddTicks(9909), false, null, "Admin", null },
                    { new Guid("d7921beb-51e3-459c-9905-c4189cfb31b9"), new DateTime(2025, 12, 1, 21, 0, 39, 452, DateTimeKind.Utc).AddTicks(9912), false, null, "User", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GroupRole",
                keyColumn: "PkGroupRoleId",
                keyValue: new Guid("31097fd3-a3c2-409d-9730-8dabffcd04a4"));

            migrationBuilder.DeleteData(
                table: "GroupRole",
                keyColumn: "PkGroupRoleId",
                keyValue: new Guid("9ca44144-2f55-4705-8753-85b8b86b7489"));

            migrationBuilder.DeleteData(
                table: "GroupRole",
                keyColumn: "PkGroupRoleId",
                keyValue: new Guid("a97a1660-afb4-4457-a4e1-bb4164afe397"));

            migrationBuilder.DeleteData(
                table: "GroupRole",
                keyColumn: "PkGroupRoleId",
                keyValue: new Guid("b7904eff-7008-4bb7-aea2-44fb27b688ff"));

            migrationBuilder.DeleteData(
                table: "UserRole",
                keyColumn: "PkUserRoleId",
                keyValue: new Guid("a96774e4-170c-4a2a-b87e-d4e3be0d4443"));

            migrationBuilder.DeleteData(
                table: "UserRole",
                keyColumn: "PkUserRoleId",
                keyValue: new Guid("d7921beb-51e3-459c-9905-c4189cfb31b9"));
        }
    }
}
