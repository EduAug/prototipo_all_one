using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace all_one_backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Topic",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TopicName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Subscribers = table.Column<int>(type: "int", nullable: false),
                    TotalVotes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topic", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Birthday = table.Column<DateTime>(type: "datetime", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(10,8)", precision: 10, scale: 8, nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(11,8)", precision: 11, scale: 8, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Friends",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false),
                    FriendID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friends", x => new { x.UserID, x.FriendID });
                    table.ForeignKey(
                        name: "FK_Friends_User_FriendID",
                        column: x => x.FriendID,
                        principalTable: "User",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Friends_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "UserTopics",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false),
                    TopicID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTopics", x => new { x.UserID, x.TopicID });
                    table.ForeignKey(
                        name: "FK_UserTopics_Topic_TopicID",
                        column: x => x.TopicID,
                        principalTable: "Topic",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_UserTopics_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TopicId = table.Column<int>(type: "int", nullable: false),
                    VoteDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => new { x.UserId, x.TopicId });
                    table.ForeignKey(
                        name: "FK_Votes_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Votes_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "FriendID",
                table: "Friends",
                column: "FriendID");

            migrationBuilder.CreateIndex(
                name: "TopicID",
                table: "UserTopics",
                column: "TopicID");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_TopicId",
                table: "Votes",
                column: "TopicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Friends");

            migrationBuilder.DropTable(
                name: "UserTopics");

            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "Topic");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
