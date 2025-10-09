using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rex.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNameToCreateChatGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FkUserNotification",
                table: "Notification");

            migrationBuilder.AddColumn<Guid>(
                name: "FkRecipientId",
                table: "Notification",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "GroupPhoto",
                table: "Chat",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverPhoto",
                table: "Challenge",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TargetId_UserId",
                table: "Reaction",
                columns: new[] { "FkTargetId", "FkUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_FkRecipientId",
                table: "Notification",
                column: "FkRecipientId");

            migrationBuilder.AddForeignKey(
                name: "FkUserReceivedNotification",
                table: "Notification",
                column: "FkRecipientId",
                principalTable: "User",
                principalColumn: "PkUserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FkUserSentNotification",
                table: "Notification",
                column: "FkUserId",
                principalTable: "User",
                principalColumn: "PkUserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FkUserReceivedNotification",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FkUserSentNotification",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_TargetId_UserId",
                table: "Reaction");

            migrationBuilder.DropIndex(
                name: "IX_Notification_FkRecipientId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "FkRecipientId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "GroupPhoto",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "CoverPhoto",
                table: "Challenge");

            migrationBuilder.AddForeignKey(
                name: "FkUserNotification",
                table: "Notification",
                column: "FkUserId",
                principalTable: "User",
                principalColumn: "PkUserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
