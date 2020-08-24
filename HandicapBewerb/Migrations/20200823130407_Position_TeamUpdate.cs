using Microsoft.EntityFrameworkCore.Migrations;

namespace TournamentManager.Migrations
{
    public partial class Position_TeamUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "TeamMatchResults",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "TeamMatchResults");
        }
    }
}
