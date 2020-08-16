using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TournamentManager.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    MatchId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.MatchId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "MatchResults",
                columns: table => new
                {
                    MatchResultId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Result = table.Column<double>(nullable: false),
                    Round = table.Column<double>(nullable: false),
                    Position = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    MatchId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchResults", x => x.MatchResultId);
                    table.ForeignKey(
                        name: "FK_MatchResults_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rounds",
                columns: table => new
                {
                    RoundId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(nullable: false),
                    Points = table.Column<double>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rounds", x => x.RoundId);
                    table.ForeignKey(
                        name: "FK_Rounds_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserMatches",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    MatchId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMatches", x => new { x.UserId, x.MatchId });
                    table.ForeignKey(
                        name: "FK_UserMatches_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMatches_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatchResults_MatchId",
                table: "MatchResults",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_UserId",
                table: "Rounds",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMatches_MatchId",
                table: "UserMatches",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Name",
                table: "Users",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchResults");

            migrationBuilder.DropTable(
                name: "Rounds");

            migrationBuilder.DropTable(
                name: "UserMatches");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
