using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rex.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chat",
                columns: table => new
                {
                    PkChatId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkChatId", x => x.PkChatId);
                });

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    PkGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Visibility = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkGroupId", x => x.PkGroupId);
                });

            migrationBuilder.CreateTable(
                name: "GroupRole",
                columns: table => new
                {
                    PkGroupRoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkGroupRoleId", x => x.PkGroupRoleId);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    PkUserRoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkUserRoleId", x => x.PkUserRoleId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    PkUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ProfilePhoto = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CoverPhoto = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Biography = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Birthday = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ConfirmedAccount = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FkRoleId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkUserId", x => x.PkUserId);
                    table.ForeignKey(
                        name: "FkUserRoleUser",
                        column: x => x.FkRoleId,
                        principalTable: "UserRole",
                        principalColumn: "PkUserRoleId");
                });

            migrationBuilder.CreateTable(
                name: "Challenge",
                columns: table => new
                {
                    PkChallengeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FkCreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkChallengeId", x => x.PkChallengeId);
                    table.ForeignKey(
                        name: "FK_Challenge_User_FkCreatorId",
                        column: x => x.FkCreatorId,
                        principalTable: "User",
                        principalColumn: "PkUserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FkGroupChallenge",
                        column: x => x.FkGroupId,
                        principalTable: "Group",
                        principalColumn: "PkGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Code",
                columns: table => new
                {
                    PkCodeId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    Expiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Revoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    RefreshCode = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkCodeId", x => x.PkCodeId);
                    table.ForeignKey(
                        name: "FkUserCode",
                        column: x => x.FkUserId,
                        principalTable: "User",
                        principalColumn: "PkUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    PkFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkFileId", x => x.PkFileId);
                    table.ForeignKey(
                        name: "FK_File_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "PkUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FriendShip",
                columns: table => new
                {
                    PkFriendShipId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkTargetUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkRequesterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkFriendShipId", x => x.PkFriendShipId);
                    table.ForeignKey(
                        name: "FkUserReceivedFriendRequest",
                        column: x => x.FkTargetUserId,
                        principalTable: "User",
                        principalColumn: "PkUserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FkUserSentFriendRequest",
                        column: x => x.FkRequesterId,
                        principalTable: "User",
                        principalColumn: "PkUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    FkChatId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkSenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkMessageId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Message_User_FkSenderId",
                        column: x => x.FkSenderId,
                        principalTable: "User",
                        principalColumn: "PkUserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FkChatMessage",
                        column: x => x.FkChatId,
                        principalTable: "Chat",
                        principalColumn: "PkChatId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    PkNotificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FkUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Read = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkNotificationId", x => x.PkNotificationId);
                    table.ForeignKey(
                        name: "FkUserNotification",
                        column: x => x.FkUserId,
                        principalTable: "User",
                        principalColumn: "PkUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reaction",
                columns: table => new
                {
                    PkReactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkTargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkReactionId", x => x.PkReactionId);
                    table.ForeignKey(
                        name: "FkUserReaction",
                        column: x => x.FkUserId,
                        principalTable: "User",
                        principalColumn: "PkUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    PkRefreshTokenId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Used = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Expiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Revoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkRefreshTokenId", x => x.PkRefreshTokenId);
                    table.ForeignKey(
                        name: "FkUserRefreshToken",
                        column: x => x.FkUserId,
                        principalTable: "User",
                        principalColumn: "PkUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserChat",
                columns: table => new
                {
                    PkUserChatId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkChatId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkUserChatId", x => x.PkUserChatId);
                    table.ForeignKey(
                        name: "FkChatUserChat",
                        column: x => x.FkChatId,
                        principalTable: "Chat",
                        principalColumn: "PkChatId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FkUserUserChat",
                        column: x => x.FkUserId,
                        principalTable: "User",
                        principalColumn: "PkUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGroup",
                columns: table => new
                {
                    PkUserGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkGroupRoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkUserGroupId", x => x.PkUserGroupId);
                    table.ForeignKey(
                        name: "FkGroupRoleUserGroup",
                        column: x => x.FkGroupRoleId,
                        principalTable: "GroupRole",
                        principalColumn: "PkGroupRoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FkGroupUserGroup",
                        column: x => x.FkGroupId,
                        principalTable: "Group",
                        principalColumn: "PkGroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FkUserGroup",
                        column: x => x.FkUserId,
                        principalTable: "User",
                        principalColumn: "PkUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    PkPostId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FkUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkChallengeId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkPostId", x => x.PkPostId);
                    table.ForeignKey(
                        name: "FkChallengePost",
                        column: x => x.FkChallengeId,
                        principalTable: "Challenge",
                        principalColumn: "PkChallengeId");
                    table.ForeignKey(
                        name: "FkGroupPost",
                        column: x => x.FkGroupId,
                        principalTable: "Group",
                        principalColumn: "PkGroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FkUserPost",
                        column: x => x.FkUserId,
                        principalTable: "User",
                        principalColumn: "PkUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityFile",
                columns: table => new
                {
                    PkEntityFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkTargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkEntityFileId", x => x.PkEntityFileId);
                    table.ForeignKey(
                        name: "FkFileEntityFile",
                        column: x => x.FkFileId,
                        principalTable: "File",
                        principalColumn: "PkFileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    PkCommentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FkPostId = table.Column<Guid>(type: "uuid", nullable: false),
                    FkUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPinned = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ParentCommentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Edited = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PkCommentId", x => x.PkCommentId);
                    table.ForeignKey(
                        name: "FkComment",
                        column: x => x.ParentCommentId,
                        principalTable: "Comment",
                        principalColumn: "PkCommentId");
                    table.ForeignKey(
                        name: "FkPostComment",
                        column: x => x.FkPostId,
                        principalTable: "Post",
                        principalColumn: "PkPostId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FkUserComment",
                        column: x => x.FkUserId,
                        principalTable: "User",
                        principalColumn: "PkUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Challenge_FkCreatorId",
                table: "Challenge",
                column: "FkCreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenge_FkGroupId",
                table: "Challenge",
                column: "FkGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Code_FkUserId",
                table: "Code",
                column: "FkUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_FkPostId",
                table: "Comment",
                column: "FkPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_FkUserId",
                table: "Comment",
                column: "FkUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ParentCommentId",
                table: "Comment",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityFile_FkFileId",
                table: "EntityFile",
                column: "FkFileId");

            migrationBuilder.CreateIndex(
                name: "IX_File_UserId",
                table: "File",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendShip_FkRequesterId",
                table: "FriendShip",
                column: "FkRequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendShip_FkTargetUserId",
                table: "FriendShip",
                column: "FkTargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_FkChatId",
                table: "Message",
                column: "FkChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_FkSenderId",
                table: "Message",
                column: "FkSenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_FkUserId",
                table: "Notification",
                column: "FkUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_FkChallengeId",
                table: "Post",
                column: "FkChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_FkGroupId",
                table: "Post",
                column: "FkGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_FkUserId",
                table: "Post",
                column: "FkUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reaction_FkUserId",
                table: "Reaction",
                column: "FkUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_FkUserId",
                table: "RefreshToken",
                column: "FkUserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_FkRoleId",
                table: "User",
                column: "FkRoleId");

            migrationBuilder.CreateIndex(
                name: "UQEmail",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQUsername",
                table: "User",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserChat_FkChatId",
                table: "UserChat",
                column: "FkChatId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChat_FkUserId",
                table: "UserChat",
                column: "FkUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroup_FkGroupId",
                table: "UserGroup",
                column: "FkGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroup_FkGroupRoleId",
                table: "UserGroup",
                column: "FkGroupRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroup_FkUserId",
                table: "UserGroup",
                column: "FkUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Code");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "EntityFile");

            migrationBuilder.DropTable(
                name: "FriendShip");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Reaction");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "UserChat");

            migrationBuilder.DropTable(
                name: "UserGroup");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "File");

            migrationBuilder.DropTable(
                name: "Chat");

            migrationBuilder.DropTable(
                name: "GroupRole");

            migrationBuilder.DropTable(
                name: "Challenge");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.DropTable(
                name: "UserRole");
        }
    }
}
