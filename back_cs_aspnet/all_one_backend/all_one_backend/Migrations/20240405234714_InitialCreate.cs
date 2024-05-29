using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace all_one_backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Topic",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    TopicName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Subscribers = table.Column<int>(type: "int", nullable: false),
                    TotalVotes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ID);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    DisplayName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Birthday = table.Column<DateTime>(type: "datetime", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(10,8)", precision: 10, scale: 8, nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(11,8)", precision: 11, scale: 8, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ID);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Friends",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false),
                    FriendID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.UserID, x.FriendID });
                    table.ForeignKey(
                        name: "Friends_ibfk_1",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "Friends_ibfk_2",
                        column: x => x.FriendID,
                        principalTable: "User",
                        principalColumn: "ID");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserTopics",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false),
                    TopicID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.UserID, x.TopicID });
                    table.ForeignKey(
                        name: "UserTopics_ibfk_1",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "UserTopics_ibfk_2",
                        column: x => x.TopicID,
                        principalTable: "Topic",
                        principalColumn: "ID");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FriendID",
                table: "Friends",
                column: "FriendID");

            migrationBuilder.CreateIndex(
                name: "TopicID",
                table: "UserTopics",
                column: "TopicID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Friends");

            migrationBuilder.DropTable(
                name: "UserTopics");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Topic");
        }
    }
}
