using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace butterystrava.Migrations
{
    public partial class AddDateRefreshedAndExpires : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateRefreshed",
                table: "Accounts",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateRefreshed",
                table: "Accounts");
        }
    }
}
