using Microsoft.EntityFrameworkCore.Migrations;

namespace butterystrava.Migrations
{
    public partial class AddExpiry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ExpiresAt",
                table: "Accounts",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "ExpiresIn",
                table: "Accounts",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ExpiresIn",
                table: "Accounts");
        }
    }
}
