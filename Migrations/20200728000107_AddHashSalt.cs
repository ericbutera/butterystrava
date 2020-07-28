using Microsoft.EntityFrameworkCore.Migrations;

namespace butterystrava.Migrations
{
    public partial class AddHashSalt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "Accounts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Salt",
                table: "Accounts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hash",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Salt",
                table: "Accounts");
        }
    }
}
