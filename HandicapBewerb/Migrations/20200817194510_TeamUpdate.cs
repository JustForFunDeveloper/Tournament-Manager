using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TournamentManager.Migrations
{
    public partial class TeamUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeamMatches",
                columns: table => new
                {
                    TeamMatchId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMatches", x => x.TeamMatchId);
                });

            migrationBuilder.CreateTable(
                name: "SoloTeamMatchResults",
                columns: table => new
                {
                    SoloTeamMatchResultId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Result = table.Column<double>(nullable: false),
                    Round = table.Column<double>(nullable: false),
                    Position = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    TeamMatchId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoloTeamMatchResults", x => x.SoloTeamMatchResultId);
                    table.ForeignKey(
                        name: "FK_SoloTeamMatchResults_TeamMatches_TeamMatchId",
                        column: x => x.TeamMatchId,
                        principalTable: "TeamMatches",
                        principalColumn: "TeamMatchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamMatchResults",
                columns: table => new
                {
                    TeamMatchResultId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Result = table.Column<double>(nullable: false),
                    TeamName = table.Column<string>(nullable: true),
                    UserNames = table.Column<string>(nullable: true),
                    TeamMatchId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMatchResults", x => x.TeamMatchResultId);
                    table.ForeignKey(
                        name: "FK_TeamMatchResults_TeamMatches_TeamMatchId",
                        column: x => x.TeamMatchId,
                        principalTable: "TeamMatches",
                        principalColumn: "TeamMatchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserTeamMatches",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    TeamMatchId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTeamMatches", x => new { x.UserId, x.TeamMatchId });
                    table.ForeignKey(
                        name: "FK_UserTeamMatches_TeamMatches_TeamMatchId",
                        column: x => x.TeamMatchId,
                        principalTable: "TeamMatches",
                        principalColumn: "TeamMatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTeamMatches_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SoloTeamMatchResults_TeamMatchId",
                table: "SoloTeamMatchResults",
                column: "TeamMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMatchResults_TeamMatchId",
                table: "TeamMatchResults",
                column: "TeamMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTeamMatches_TeamMatchId",
                table: "UserTeamMatches",
                column: "TeamMatchId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoloTeamMatchResults");

            migrationBuilder.DropTable(
                name: "TeamMatchResults");

            migrationBuilder.DropTable(
                name: "UserTeamMatches");

            migrationBuilder.DropTable(
                name: "TeamMatches");
        }
    }
}
