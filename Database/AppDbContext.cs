using Database.Models;
using Microsoft.Data.SqlClient;

namespace Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    private AppDbContext() : base()
    {
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Chat> Chats { get; set; }
    public virtual DbSet<PrivateChat> PrivateChats { get; set; }
    public virtual DbSet<GroupChat> GroupChats { get; set; }
    public virtual DbSet<GroupChatUser> GroupChatUsers { get; set; }
    public virtual DbSet<ChatEvent> ChatEvents { get; set; }
    protected virtual DbSet<VerificationResult> CheckUserAndChatExistenceResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        modelBuilder.Entity<VerificationResult>().HasNoKey();
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        //var user1 = new User(new UserId(Guid.Parse("00AD8832-1C8E-4C5E-B0F5-338B619D62F7")), new UserName("User1"), new Email("User1@email.com"), new AuthorizationVersion(1));
        //var user2 = new User(new UserId(Guid.Parse("8AEBFC91-EBE4-4080-B9F9-3F1C8312DEB3")), new UserName("User2"), new Email("User2@email.com"), new AuthorizationVersion(1));
        //var user3 = new User(new UserId(Guid.Parse("5DEFEB16-A804-4F86-A5EE-1BFE93D37853")), new UserName("User3"), new Email("User3@email.com"), new AuthorizationVersion(1));
        //modelBuilder.Entity<User>().HasData(user1, user2, user3);


        //var privateChat = new PrivateChat(new ChatId(Guid.Parse("DC306013-C6F4-4158-84CC-601BC348C13A")), user1.Id, user2.Id);
        //modelBuilder.Entity<PrivateChat>().HasData(privateChat);


        //var groupChat = new GroupChat(new ChatId(Guid.Parse("A7373D57-3ABB-4C53-AB39-23A3FAA4A2DA")),
        //    new ChatName("Band of 3"));
        //modelBuilder.Entity<GroupChat>().HasData(groupChat);

        //List<GroupChatUser> groupChatUsers =
        //[
        //    new GroupChatUser(groupChat.Id, user1.Id, GroupChatRole.Owner),
        //    new GroupChatUser(groupChat.Id, user2.Id, GroupChatRole.Manager),
        //    new GroupChatUser(groupChat.Id, user3.Id, GroupChatRole.User)
        //];
        //modelBuilder.Entity<GroupChatUser>().HasData(groupChatUsers);


        //var privateMessage = new Message(MessageId.Of(Guid.Parse("568A44AE-3AD3-4FED-BD9B-1ED5A82705E7")), user1.Id, privateChat.Id, TextContent.Of("Hello"), MessageStatus.Sent, new DateTime(2024, 12, 14, 10, 42, 10, DateTimeKind.Utc), null);
        //modelBuilder.Entity<Message>().HasData(privateMessage);

        //List<Message> messagesToGroup =
        //[
        //    new Message(new MessageId(Guid.Parse("3AA8D0BE-8A87-42E7-9E71-29B639A309B7")),
        //        user1.Id, 
        //        groupChat.Id,
        //        new TextContent("Hi, band!"), MessageStatus.Read,
        //        new DateTime(2024, 12, 14, 20, 20, 20, DateTimeKind.Utc), null),
        //    new Message(new MessageId(Guid.Parse("45445420-71C5-450C-977B-B7BCFB778F7F")),
        //        user2.Id, 
        //        groupChat.Id,
        //        new TextContent("Hi, Bob!"), MessageStatus.Sent,
        //        new DateTime(2024, 12, 14, 21, 20, 20, DateTimeKind.Utc),
        //        new DateTime(2024, 12, 14, 21, 21, 20, DateTimeKind.Utc))
        //];
        //modelBuilder.Entity<Message>().HasData(messagesToGroup);


    }
    public async Task<VerificationResult> CheckUserAndChatExistenceAsync(UserId senderId, ChatId receiverId, CancellationToken cancellationToken)
    {
        var senderIdParam = new SqlParameter("@SenderId", senderId.Value);
        var receiverIdParam = new SqlParameter("@ReceiverId", receiverId.Value);

        var result = this.Set<VerificationResult>()
            .FromSqlRaw("EXEC CheckUserAndChatExistence @SenderId, @ReceiverId", senderIdParam, receiverIdParam)
            .AsNoTracking()
            .AsEnumerable()
            .SingleOrDefault();
        return result ?? new VerificationResult { UserExists = false, ChatExists = false, IsMemberOfChat = false };
    }

    public void EnsureCreated()
    {
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
    }
}

public class VerificationResult
{
    public bool UserExists { get; set; }
    public bool ChatExists { get; set; }
    public bool IsMemberOfChat { get; set; }
}