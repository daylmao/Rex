using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rex.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "UQUsername",
                table: "User",
                newName: "UQ_User_UserName");

            migrationBuilder.RenameIndex(
                name: "UQGitHubId",
                table: "User",
                newName: "UQ_User_GitHubId");

            migrationBuilder.RenameIndex(
                name: "UQEmail",
                table: "User",
                newName: "UQ_User_Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "UQ_User_UserName",
                table: "User",
                newName: "UQUsername");

            migrationBuilder.RenameIndex(
                name: "UQ_User_GitHubId",
                table: "User",
                newName: "UQGitHubId");

            migrationBuilder.RenameIndex(
                name: "UQ_User_Email",
                table: "User",
                newName: "UQEmail");
        }
    }
}
