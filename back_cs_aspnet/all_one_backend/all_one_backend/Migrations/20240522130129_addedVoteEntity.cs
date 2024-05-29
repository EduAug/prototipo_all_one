using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace all_one_backend.Migrations
{
    /// <inheritdoc />
    public partial class addedVoteEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                        name: "Votes_ibfk_1",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "Votes_ibfk_2",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "ID");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_TopicId",
                table: "Votes",
                column: "TopicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Votes");
        }
    }
}
