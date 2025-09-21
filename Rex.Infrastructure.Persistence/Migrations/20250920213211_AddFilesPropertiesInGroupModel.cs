using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rex.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFilesPropertiesInGroupModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverPhoto",
                table: "Group",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePhoto",
                table: "Group",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverPhoto",
                table: "Group");

            migrationBuilder.DropColumn(
                name: "ProfilePhoto",
                table: "Group");
        }
    }
}
