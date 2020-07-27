using Microsoft.EntityFrameworkCore.Migrations;

namespace butterystrava.Migrations
{
    public partial class AddAthleteUsernameAndId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AthleteId",
                table: "Accounts",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "AthleteUsername",
                table: "Accounts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AthleteId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "AthleteUsername",
                table: "Accounts");
        }
    }
}
