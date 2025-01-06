using Database.Models;
using Microsoft.Data.SqlClient;

namespace Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public AppDbContext()
    {
    }

    public virtual DbSet<User> Users { get; init; }
    public virtual DbSet<Chat> Chats { get; init; }
    public virtual DbSet<PrivateChat> PrivateChats { get; init; }
    public virtual DbSet<GroupChat> GroupChats { get; init; }
    public virtual DbSet<GroupChatUser> GroupChatUsers { get; init; }
    public virtual DbSet<ChatEvent> ChatEvents { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        //modelBuilder.Entity<VerificationResult>().HasNoKey();
        //modelBuilder.Entity<UserId>().HasNoKey();
    }
    public virtual async Task<VerificationResult> CheckUserAndChatExistenceAsync(UserId senderId, ChatId receiverId, CancellationToken cancellationToken)
    {
        //var senderIdParam = new SqlParameter("@SenderId", senderId.Value);
        //var receiverIdParam = new SqlParameter("@ReceiverId", receiverId.Value);

        var result = await Database
            .SqlQuery<VerificationResult>($"EXEC CheckUserAndChatExistence {senderId.Value}, {receiverId.Value}")
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return result.Single();
    }

    public virtual async Task<List<UserId>> GetChatMembersAsync(ChatId chatId, CancellationToken cancellationToken)
    {
        var result = await Database
            .SqlQuery<UserId>($"EXEC GetChatMembers {chatId.Value}")
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return result;
    }

    public void RecreateDatabase()
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();
        Database.ExecuteSql($"""
                             CREATE PROCEDURE CheckUserAndChatExistence 
                                 @SenderId uniqueidentifier, 
                                 @ReceiverId uniqueidentifier
                             AS
                             BEGIN
                                 SELECT 
                                     CAST(
                                         CASE 
                                             WHEN EXISTS(SELECT 1 FROM Users WHERE Id = @SenderId) 
                                             THEN 1 
                                             ELSE 0 
                                         END AS bit
                                     ) AS UserExists,
                                     
                                     CAST(
                                         CASE 
                                             WHEN EXISTS(SELECT 1 FROM PrivateChats WHERE Id = @ReceiverId OR UserId1 = @SenderId OR UserId2 = @SenderId) 
                                             OR EXISTS(SELECT 1 FROM GroupChats WHERE Id = @ReceiverId) 
                                             THEN 1 
                                             ELSE 0 
                                         END AS bit
                                     ) AS ChatExists,
                                     
                                     CAST(
                                         CASE 
                                             WHEN EXISTS(SELECT 1 FROM GroupChatUsers WHERE UserId = @SenderId AND GroupChatId = @ReceiverId) 
                                             OR EXISTS(SELECT 1 FROM PrivateChats WHERE Id = @ReceiverId AND (UserId1 = @SenderId OR UserId2 = @SenderId)) 
                                             THEN 1 
                                             ELSE 0 
                                         END AS bit
                                     ) AS IsMemberOfChat
                             END
                             """);
        Database.ExecuteSql($"""
                             CREATE PROCEDURE GetChatMembers 
                                 @ChatId uniqueidentifier
                             AS
                             BEGIN
                                 SELECT UserId
                                 FROM GroupChatUsers
                                 WHERE GroupChatId = @ChatId
                                 UNION
                                 SELECT UserId1
                                 FROM PrivateChats
                                 WHERE Id = @ChatId
                                 UNION
                                 SELECT UserId2
                                 FROM PrivateChats
                                 WHERE Id = @ChatId
                             END
                             """);
    }
}

public class VerificationResult
{
    public bool UserExists { get; set; }
    public bool ChatExists { get; set; }
    public bool IsMemberOfChat { get; set; }
}

