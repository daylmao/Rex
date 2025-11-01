using Microsoft.EntityFrameworkCore;
using Rex.Models;
using File = Rex.Models.File;
using GroupRole = Rex.Models.GroupRole;
using UserRole = Rex.Models.UserRole;

namespace Rex.Infrastructure.Persistence.Context;

public class RexContext: DbContext
{
    public RexContext(DbContextOptions<RexContext> options) : base(options) {}

    #region Models

    public DbSet<User> User { get; set; }
    public DbSet<RefreshToken> RefreshToken { get; set; }
    public DbSet<FriendShip> FriendShip { get; set; }
    public DbSet<Code> Code { get; set; }
    public DbSet<UserRole> UserRole { get; set; }
    public DbSet<GroupRole> GroupRole { get; set; }
    public DbSet<UserGroup> UserGroup { get; set; }
    public DbSet<Group> Group { get; set; }
    public DbSet<Post> Post { get; set; }
    public DbSet<EntityFile> EntityFile { get; set; }
    public DbSet<File> File { get; set; }
    public DbSet<Challenge> Challenge { get; set; }
    public DbSet<Comment> Comment { get; set; }
    public DbSet<Reaction> Reaction { get; set; }
    public DbSet<Chat> Chat { get; set; }
    public DbSet<UserChat> UserChat { get; set; }
    public DbSet<Message> Message { get; set; }
    public DbSet<Notification> Notification { get; set; }
    
    #endregion
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region Indexes
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("UQ_User_Email");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasIndex(p => new { p.GroupId, p.CreatedAt })
                .HasDatabaseName("IX_Post_GroupId_CreatedAt");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasIndex(m => new { m.ChatId, m.CreatedAt })
                .HasDatabaseName("IX_Message_ChatId_CreatedAt");
        });

        modelBuilder.Entity<UserGroup>(entity =>
        {
            entity.HasIndex(ug => new { ug.UserId, ug.GroupId, ug.Status })
                .HasDatabaseName("IX_UserGroup_UserId_GroupId_Status");
        });

        modelBuilder.Entity<Reaction>(entity =>
        {
            entity.HasIndex(r => new { r.TargetId, r.TargetType, r.Like })
                .HasDatabaseName("IX_Reaction_TargetId_Type_Like")
                .HasFilter("\"Like\" = true");
        });

        modelBuilder.Entity<FriendShip>(entity =>
        {
            entity.HasIndex(f => new { f.RequesterId, f.TargetUserId, f.Status })
                .HasDatabaseName("IX_FriendShip_RequesterId_TargetUserId_Status");
        });

        modelBuilder.Entity<Code>(entity =>
        {
            entity.HasIndex(c => new { c.UserId, c.Type, c.Used, c.Revoked })
                .HasDatabaseName("IX_Code_UserId_Type_Status");
        });

        #endregion

        #region Tables

        modelBuilder.Entity<User>()
            .ToTable("User");

        modelBuilder.Entity<RefreshToken>()
            .ToTable("RefreshToken");

        modelBuilder.Entity<FriendShip>()
            .ToTable("FriendShip");

        modelBuilder.Entity<Code>()
            .ToTable("Code");

        modelBuilder.Entity<UserRole>()
            .ToTable("UserRole");

        modelBuilder.Entity<GroupRole>()
            .ToTable("GroupRole");

        modelBuilder.Entity<UserGroup>()
            .ToTable("UserGroup");

        modelBuilder.Entity<Group>()
            .ToTable("Group");

        modelBuilder.Entity<Post>()
            .ToTable("Post");

        modelBuilder.Entity<EntityFile>()
            .ToTable("EntityFile");

        modelBuilder.Entity<File>()
            .ToTable("File");

        modelBuilder.Entity<Challenge>()
            .ToTable("Challenge");

        modelBuilder.Entity<Comment>()
            .ToTable("Comment");

        modelBuilder.Entity<Reaction>()
            .ToTable("Reaction");

        modelBuilder.Entity<Chat>()
            .ToTable("Chat");

        modelBuilder.Entity<UserChat>()
            .ToTable("UserChat");

        modelBuilder.Entity<Message>()
            .ToTable("Message");

        modelBuilder.Entity<Notification>()
            .ToTable("Notification");

        #endregion

        #region Primary Key

        modelBuilder.Entity<User>()
            .HasKey(u => u.Id)
            .HasName("PkUserId");

        modelBuilder.Entity<Challenge>()
            .HasKey(c => c.Id)
            .HasName("PkChallengeId");

        modelBuilder.Entity<Chat>()
            .HasKey(c => c.Id)
            .HasName("PkChatId");

        modelBuilder.Entity<Code>()
            .HasKey(c => c.Id)
            .HasName("PkCodeId");

        modelBuilder.Entity<Comment>()
            .HasKey(c => c.Id)
            .HasName("PkCommentId");

        modelBuilder.Entity<EntityFile>()
            .HasKey(c => c.Id)
            .HasName("PkEntityFileId");

        modelBuilder.Entity<File>()
            .HasKey(c => c.Id)
            .HasName("PkFileId");

        modelBuilder.Entity<FriendShip>()
            .HasKey(c => c.Id)
            .HasName("PkFriendShipId");

        modelBuilder.Entity<Group>()
            .HasKey(c => c.Id)
            .HasName("PkGroupId");

        modelBuilder.Entity<GroupRole>()
            .HasKey(c => c.Id)
            .HasName("PkGroupRoleId");

        modelBuilder.Entity<Message>()
            .HasKey(c => c.Id)
            .HasName("PkMessageId");

        modelBuilder.Entity<Notification>()
            .HasKey(c => c.Id)
            .HasName("PkNotificationId");

        modelBuilder.Entity<Post>()
            .HasKey(c => c.Id)
            .HasName("PkPostId");

        modelBuilder.Entity<Reaction>()
            .HasKey(c => c.Id)
            .HasName("PkReactionId");

        modelBuilder.Entity<RefreshToken>()
            .HasKey(c => c.Id)
            .HasName("PkRefreshTokenId");

        modelBuilder.Entity<UserChat>()
            .HasKey(c => c.Id)
            .HasName("PkUserChatId");

        modelBuilder.Entity<UserGroup>()
            .HasKey(c => c.Id)
            .HasName("PkUserGroupId");

        modelBuilder.Entity<UserRole>()
            .HasKey(c => c.Id)
            .HasName("PkUserRoleId");

        #endregion

        #region Foreign Keys

        modelBuilder.Entity<User>()
            .Property(c => c.RoleId)
            .HasColumnName("FkRoleId");

        modelBuilder.Entity<Challenge>()
            .Property(c => c.CreatorId)
            .HasColumnName("FkCreatorId");

        modelBuilder.Entity<Challenge>()
            .Property(c => c.GroupId)
            .HasColumnName("FkGroupId");

        modelBuilder.Entity<Code>()
            .Property(c => c.UserId)
            .HasColumnName("FkUserId");

        modelBuilder.Entity<Comment>()
            .Property(c => c.UserId)
            .HasColumnName("FkUserId");

        modelBuilder.Entity<Comment>()
            .Property(c => c.PostId)
            .HasColumnName("FkPostId");

        modelBuilder.Entity<EntityFile>()
            .Property(c => c.FileId)
            .HasColumnName("FkFileId");

        modelBuilder.Entity<EntityFile>()
            .Property(c => c.TargetId)
            .HasColumnName("FkTargetId");

        modelBuilder.Entity<FriendShip>()
            .Property(c => c.TargetUserId)
            .HasColumnName("FkTargetUserId");

        modelBuilder.Entity<FriendShip>()
            .Property(c => c.RequesterId)
            .HasColumnName("FkRequesterId");

        modelBuilder.Entity<Message>()
            .Property(c => c.ChatId)
            .HasColumnName("FkChatId");

        modelBuilder.Entity<Message>()
            .Property(c => c.SenderId)
            .HasColumnName("FkSenderId");

        modelBuilder.Entity<Notification>()
            .Property(c => c.UserId)
            .HasColumnName("FkUserId");

        modelBuilder.Entity<Post>()
            .Property(c => c.UserId)
            .HasColumnName("FkUserId");

        modelBuilder.Entity<Post>()
            .Property(c => c.GroupId)
            .HasColumnName("FkGroupId");

        modelBuilder.Entity<Post>()
            .Property(c => c.ChallengeId)
            .HasColumnName("FkChallengeId");

        modelBuilder.Entity<Reaction>()
            .Property(c => c.UserId)
            .HasColumnName("FkUserId");

        modelBuilder.Entity<Reaction>()
            .Property(c => c.TargetId)
            .HasColumnName("FkTargetId");

        modelBuilder.Entity<RefreshToken>()
            .Property(c => c.UserId)
            .HasColumnName("FkUserId");

        modelBuilder.Entity<UserChat>()
            .Property(c => c.ChatId)
            .HasColumnName("FkChatId");

        modelBuilder.Entity<UserChat>()
            .Property(c => c.UserId)
            .HasColumnName("FkUserId");

        modelBuilder.Entity<UserGroup>()
            .Property(c => c.UserId)
            .HasColumnName("FkUserId");

        modelBuilder.Entity<UserGroup>()
            .Property(c => c.GroupId)
            .HasColumnName("FkGroupId");

        modelBuilder.Entity<UserGroup>()
            .Property(c => c.GroupRoleId)
            .HasColumnName("FkGroupRoleId");

        #endregion

        #region Relationships

        modelBuilder.Entity<User>()
            .HasMany(c => c.RefreshTokens)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .HasConstraintName("FkUserRefreshToken");

        modelBuilder.Entity<User>()
            .HasMany(c => c.Codes)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .HasConstraintName("FkUserCode");

        modelBuilder.Entity<User>()
            .HasMany(c => c.Posts)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .HasConstraintName("FkUserPost");

        modelBuilder.Entity<User>()
            .HasMany(c => c.Comments)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .HasConstraintName("FkUserComment");

        modelBuilder.Entity<User>()
            .HasMany(c => c.Reactions)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .HasConstraintName("FkUserReaction");

        modelBuilder.Entity<User>()
            .HasMany(c => c.UserGroups)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .HasConstraintName("FkUserGroup");

        modelBuilder.Entity<User>()
            .HasMany(c => c.UserChats)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .HasConstraintName("FkUserUserChat");

        modelBuilder.Entity<User>()
            .HasMany(c => c.SentNotifications)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .HasConstraintName("FkUserSentNotification");

        modelBuilder.Entity<User>()
            .HasMany(c => c.SentFriendRequests)
            .WithOne(c => c.Requester)
            .HasForeignKey(c => c.RequesterId)
            .HasConstraintName("FkUserSentFriendRequest");

        modelBuilder.Entity<User>()
            .HasMany(c => c.ReceivedFriendRequests)
            .WithOne(c => c.TargetUser)
            .HasForeignKey(c => c.TargetUserId)
            .HasConstraintName("FkUserReceivedFriendRequest");

        modelBuilder.Entity<Challenge>()
            .HasMany(c => c.Posts)
            .WithOne(c => c.Challenge)
            .HasForeignKey(c => c.ChallengeId)
            .HasConstraintName("FkChallengePost");

        modelBuilder.Entity<Chat>()
            .HasMany(c => c.UserChats)
            .WithOne(c => c.Chat)
            .HasForeignKey(c => c.ChatId)
            .HasConstraintName("FkChatUserChat");

        modelBuilder.Entity<Chat>()
            .HasMany(c => c.Messages)
            .WithOne(c => c.Chat)
            .HasForeignKey(c => c.ChatId)
            .HasConstraintName("FkChatMessage");

        modelBuilder.Entity<Comment>()
            .HasMany(c => c.Replies)
            .WithOne(c => c.ParentComment)
            .HasForeignKey(c => c.ParentCommentId)
            .HasConstraintName("FkComment");

        modelBuilder.Entity<File>()
            .HasMany(c => c.EntityFiles)
            .WithOne(c => c.File)
            .HasForeignKey(c => c.FileId)
            .HasConstraintName("FkFileEntityFile");

        modelBuilder.Entity<Group>()
            .HasMany(c => c.Posts)
            .WithOne(c => c.Group)
            .HasForeignKey(c => c.GroupId)
            .HasConstraintName("FkGroupPost");

        modelBuilder.Entity<Group>()
            .HasMany(c => c.UserGroups)
            .WithOne(c => c.Group)
            .HasForeignKey(c => c.GroupId)
            .HasConstraintName("FkGroupUserGroup");

        modelBuilder.Entity<Group>()
            .HasMany(c => c.Challenges)
            .WithOne(c => c.Group)
            .HasForeignKey(c => c.GroupId)
            .HasConstraintName("FkGroupChallenge");

        modelBuilder.Entity<GroupRole>()
            .HasMany(c => c.UserGroups)
            .WithOne(c => c.GroupRole)
            .HasForeignKey(c => c.GroupRoleId)
            .HasConstraintName("FkGroupRoleUserGroup");

        modelBuilder.Entity<Post>()
            .HasMany(c => c.Comments)
            .WithOne(c => c.Post)
            .HasForeignKey(c => c.PostId)
            .HasConstraintName("FkPostComment");

        modelBuilder.Entity<UserRole>()
            .HasMany(c => c.Users)
            .WithOne(c => c.Role)
            .HasForeignKey(c => c.RoleId)
            .HasConstraintName("FkUserRoleUser");

        #endregion

        #region User

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(a => a.Id)
                .HasColumnName("PkUserId")
                .IsRequired();
    
            entity.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(a => a.Email)
                .IsRequired();

            entity.Property(u => u.Password)
                .IsRequired(false)
                .HasMaxLength(255);

            entity.Property(u => u.ProfilePhoto)
                .IsRequired(false)
                .HasMaxLength(255);

            entity.Property(u => u.CoverPhoto)
                .IsRequired(false)
                .HasMaxLength(255);

            entity.Property(u => u.Biography)
                .IsRequired(false)
                .HasColumnType("text");

            entity.Property(u => u.ConfirmedAccount)
                .HasDefaultValue(false);
    
            entity.Property(u => u.IsActive)
                .HasDefaultValue(true);

            entity.Property(u => u.Birthday)
                .IsRequired(false)
                .HasColumnType("date");

            entity.Property(u => u.LastLoginAt)
                .IsRequired(false)
                .HasColumnType("timestamptz");
    
            entity.Property(u => u.LastConnection)
                .IsRequired(false)
                .HasColumnType("timestamptz");

            entity.Property(u => u.Status)
                .IsRequired(false)
                .HasMaxLength(50);
    
            entity.Property(u => u.Gender)
                .IsRequired(false)
                .HasMaxLength(50);
    
            entity.Property(u => u.GitHubId)
                .IsRequired(false)
                .HasMaxLength(100);
        });

        #endregion

        #region Post

        modelBuilder.Entity<Post>(entity =>
        {
            entity.Property(a => a.Id)
                .HasColumnName("PkPostId")
                .IsRequired();
            
            entity.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.Description)
                .HasMaxLength(500);
        });

        #endregion

        #region Comment

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.Property(a => a.Id)
                .HasColumnName("PkCommentId")
                .IsRequired();
            
            entity.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(c => c.IsPinned)
                .HasDefaultValue(false);

            entity.Property(c => c.Edited)
                .HasDefaultValue(false);
        });

        #endregion

        #region File

        modelBuilder.Entity<File>(entity =>
        {
            entity.Property(a => a.Id)
                .HasColumnName("PkFileId")
                .IsRequired();
            
            entity.Property(f => f.Url)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(f => f.Type)
                .IsRequired()
                .HasMaxLength(30);
            
            entity.Property(u => u.UploadedAt)
                .HasColumnType("timestamptz");

        });

        #endregion

        #region EntityFile

        modelBuilder.Entity<EntityFile>(entity =>
        {
            entity.Property(a => a.Id)
                .HasColumnName("PkEntityFileId")
                .IsRequired();

            entity.Property(e => e.TargetType)
                .IsRequired()
                .HasMaxLength(50);
        });

        #endregion

        #region Challenge

        modelBuilder.Entity<Challenge>(entity =>
        {
            entity.Property(a => a.Id)
                .HasColumnName("PkChallengeId")
                .IsRequired();
            
            entity.Property(u => u.CoverPhoto)
                .HasMaxLength(255);
            
            entity.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(c => c.Description)
                .HasMaxLength(500);

            entity.Property(c => c.Duration)
                .IsRequired();

            entity.Property(c => c.Status)
                .HasMaxLength(50);
        });

        #endregion

        #region Chat

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.Property(a => a.Id)
                .HasColumnName("PkChatId")
                .IsRequired();

            entity.Property(c => c.Type)
                .IsRequired()
                .HasMaxLength(30);
            
            entity.Property(a => a.Name)
                .HasMaxLength(100);
            
            entity.Property(a => a.GroupPhoto)
                .HasMaxLength(255);
        });

        #endregion

        #region Code

        modelBuilder.Entity<Code>(entity =>
        {  
            entity.Property(a => a.Id)
                .HasColumnName("PkCodeId")
                .IsRequired();
            
            entity.Property(c => c.Value)
                .IsRequired()
                .HasMaxLength(6);

            entity.Property(c => c.Expiration)
                .IsRequired();
            
            entity.Property(c => c.Type)
                .IsRequired()
                .HasMaxLength(30);
            
            entity.Property(c => c.Expiration)
                .HasColumnType("timestamptz");

            entity.Property(c => c.Revoked)
                .HasDefaultValue(false);
            
            entity.Property(c => c.Used)
                .HasDefaultValue(false);
        });

        #endregion

        #region Notification

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(a => a.Id)
                .HasColumnName("PkNotificationId")
                .IsRequired();

            entity.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(125);

            entity.Property(n => n.Description)
                .HasMaxLength(255);

            entity.Property(n => n.Read)
                .HasDefaultValue(false);

            entity.Property(n => n.RecipientType)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(n => n.RecipientId)
                .IsRequired();

            entity.Property(n => n.MetadataJson)
                .HasColumnType("jsonb"); 
        });


        #endregion

        #region Reaction

        modelBuilder.Entity<Reaction>(entity =>
        {
            entity.Property(a => a.Id)
                .HasColumnName("PkReactionId")
                .IsRequired();

            entity.Property(r => r.TargetType)
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(a => a.Like)
                .HasDefaultValue(false);
        });

        #endregion

        #region RefreshToken

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.Property(a => a.Id)
                .HasColumnName("PkRefreshTokenId")
                .IsRequired();
            
            entity.Property(r => r.Value)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(r => r.Used)
                .HasDefaultValue(false);

            entity.Property(r => r.Revoked)
                .HasDefaultValue(false);
            
            entity.Property(r => r.Expiration)
                .HasColumnType("timestamptz");
            
        });

        #endregion

        #region UserChat

        modelBuilder.Entity<UserChat>(entity =>
        {
            entity.Property(a => a.Id)
                .HasColumnName("PkUserChatId")
                .IsRequired();
            
            entity.Property(uc => uc.ChatId)
                .IsRequired();

            entity.Property(uc => uc.UserId)
                .IsRequired();
        });

        #endregion

        #region UserGroup

        modelBuilder.Entity<UserGroup>(entity =>
        {
            entity.Property(a => a.Id)
                .HasColumnName("PkUserGroupId")
                .IsRequired();
            
            entity.Property(ug => ug.UserId)
                .IsRequired();

            entity.Property(ug => ug.GroupId)
                .IsRequired();

            entity.Property(ug => ug.GroupRoleId)
                .IsRequired();
            
            entity.Property(ug => ug.RequestedAt)
                .HasColumnType("timestamptz");
            
            entity.Property(ug => ug.CreatedAt)
                .HasColumnType("timestamptz");

            entity.Property(ug => ug.Status)
                .HasMaxLength(50);
        });

        #endregion

        #region FriendShip

        modelBuilder.Entity<FriendShip>(entity =>
        {
            entity.Property(a => a.Id)
                .HasColumnName("PkFriendShipId")
                .IsRequired();
            
            entity.Property(f => f.RequesterId)
                .IsRequired();

            entity.Property(f => f.TargetUserId)
                .IsRequired();

            entity.Property(f => f.Status)
                .IsRequired()
                .HasMaxLength(50);
        });

        #endregion

        #region Group

        modelBuilder.Entity<Group>(entity =>
        {
            entity.Property(a => a.Id)
                .HasColumnName("PkGroupId")
                .IsRequired();
            
            entity.Property(g => g.Title)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(g => g.Description)
                .HasMaxLength(500);

            entity.Property(g => g.Visibility)
                .IsRequired()
                .HasMaxLength(30);
            
            entity.Property(u => u.ProfilePhoto)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(u => u.CoverPhoto)
                .HasMaxLength(255);
        });

        #endregion

        #region GroupRole

        modelBuilder.Entity<GroupRole>(entity =>
        {
            entity.Property(a => a.Id)
                .HasColumnName("PkGroupRoleId")
                .IsRequired();
            
            entity.Property(gr => gr.Role)
                .IsRequired()
                .HasMaxLength(50);
        });

        #endregion

        #region UserRole

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.Property(a => a.Id)
                .HasColumnName("PkUserRoleId")
                .IsRequired();
            
            entity.Property(ur => ur.Role)
                .IsRequired()
                .HasMaxLength(50);
        });

        #endregion

        #region Query Filters

        modelBuilder.Entity<Chat>()
            .HasQueryFilter(c => !c.Deleted);

        modelBuilder.Entity<Comment>()
            .HasQueryFilter(c => !c.Deleted);
        
        modelBuilder.Entity<Message>()
            .HasQueryFilter(m => !m.Deleted);
        
        modelBuilder.Entity<Post>()
            .HasQueryFilter(p => !p.Deleted);
        
        modelBuilder.Entity<Reaction>()
            .HasQueryFilter(r => !r.Deleted);
        
        modelBuilder.Entity<User>()
            .HasQueryFilter(u => !u.Deleted);

        modelBuilder.Entity<Group>()
            .HasQueryFilter(g => !g.Deleted);
        
        modelBuilder.Entity<Challenge>()
            .HasQueryFilter(c => !c.Deleted);
        
        modelBuilder.Entity<EntityFile>()
            .HasQueryFilter(c => !c.Deleted);
        
        modelBuilder.Entity<FriendShip>()
            .HasQueryFilter(f => !f.Deleted && !f.Requester.Deleted && !f.TargetUser.Deleted);
        
        modelBuilder.Entity<Notification>()
            .HasQueryFilter(n => !n.Deleted && !n.User.Deleted);

        modelBuilder.Entity<RefreshToken>()
            .HasQueryFilter(r => !r.Deleted && !r.User.Deleted);

        modelBuilder.Entity<UserGroup>()
            .HasQueryFilter(ug => !ug.Deleted && !ug.User.Deleted);

        modelBuilder.Entity<UserChallenge>()
            .HasQueryFilter(uc => !uc.Deleted && !uc.Challenge.Deleted);

        modelBuilder.Entity<UserChat>()
            .HasQueryFilter(uc => !uc.Deleted && !uc.Chat.Deleted);
        
        modelBuilder.Entity<File>()
            .HasQueryFilter(c => !c.Deleted);
        
        modelBuilder.Entity<EntityFile>()
            .HasQueryFilter(c => !c.Deleted);

        #endregion
    }
    
}