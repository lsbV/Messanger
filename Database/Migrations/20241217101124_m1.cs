using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class m1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupChats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupChats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorizationVersion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupChatUser",
                columns: table => new
                {
                    GroupChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupChatUser", x => new { x.GroupChatId, x.UserId, x.Role });
                    table.ForeignKey(
                        name: "FK_GroupChatUser_GroupChats_GroupChatId",
                        column: x => x.GroupChatId,
                        principalTable: "GroupChats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupChatUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrivateChats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId2 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateChats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivateChats_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PrivateChats_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PrivateChats_Users_UserId2",
                        column: x => x.UserId2,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "GroupChats",
                columns: new[] { "Id", "ChatName" },
                values: new object[] { new Guid("a7373d57-3abb-4c53-ab39-23a3faa4a2da"), "Band of 3" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AuthorizationVersion", "Email", "Name" },
                values: new object[,]
                {
                    { new Guid("00ad8832-1c8e-4c5e-b0f5-338b619d62f7"), 1, "User1@email.com", "User1" },
                    { new Guid("5defeb16-a804-4f86-a5ee-1bfe93d37853"), 1, "User3@email.com", "User3" },
                    { new Guid("8aebfc91-ebe4-4080-b9f9-3f1c8312deb3"), 1, "User2@email.com", "User2" }
                });

            migrationBuilder.InsertData(
                table: "GroupChatUser",
                columns: new[] { "GroupChatId", "Role", "UserId" },
                values: new object[,]
                {
                    { new Guid("a7373d57-3abb-4c53-ab39-23a3faa4a2da"), "Owner", new Guid("00ad8832-1c8e-4c5e-b0f5-338b619d62f7") },
                    { new Guid("a7373d57-3abb-4c53-ab39-23a3faa4a2da"), "User", new Guid("5defeb16-a804-4f86-a5ee-1bfe93d37853") },
                    { new Guid("a7373d57-3abb-4c53-ab39-23a3faa4a2da"), "User", new Guid("8aebfc91-ebe4-4080-b9f9-3f1c8312deb3") }
                });

            migrationBuilder.InsertData(
                table: "PrivateChats",
                columns: new[] { "Id", "UserId", "UserId1", "UserId2" },
                values: new object[] { new Guid("dc306013-c6f4-4158-84cc-601bc348c13a"), null, new Guid("00ad8832-1c8e-4c5e-b0f5-338b619d62f7"), new Guid("8aebfc91-ebe4-4080-b9f9-3f1c8312deb3") });

            migrationBuilder.CreateIndex(
                name: "IX_GroupChatUser_UserId",
                table: "GroupChatUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateChats_UserId",
                table: "PrivateChats",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateChats_UserId1",
                table: "PrivateChats",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateChats_UserId2",
                table: "PrivateChats",
                column: "UserId2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupChatUser");

            migrationBuilder.DropTable(
                name: "PrivateChats");

            migrationBuilder.DropTable(
                name: "GroupChats");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
