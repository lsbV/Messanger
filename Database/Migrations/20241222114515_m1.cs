using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class m1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    event_type = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    ChatDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ChatImage = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ChatName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    joined_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    left_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupChats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChatName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ChatDescription = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ChatImage = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    JoinMode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
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
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AuthorizationVersion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupChatUsers",
                columns: table => new
                {
                    GroupChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupChatRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupChatUsers", x => new { x.GroupChatId, x.UserId });
                    table.ForeignKey(
                        name: "FK_GroupChatUsers_GroupChats_GroupChatId",
                        column: x => x.GroupChatId,
                        principalTable: "GroupChats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupChatUsers_Users_UserId",
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_ChatEvents_ChatId",
                table: "ChatEvents",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatEvents_CreatedAt",
                table: "ChatEvents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_GroupChatUsers_UserId",
                table: "GroupChatUsers",
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
                name: "ChatEvents");

            migrationBuilder.DropTable(
                name: "GroupChatUsers");

            migrationBuilder.DropTable(
                name: "PrivateChats");

            migrationBuilder.DropTable(
                name: "GroupChats");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
