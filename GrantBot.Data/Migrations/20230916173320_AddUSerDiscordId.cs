using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrantBot.Data.Migrations
{
    public partial class AddUSerDiscordId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscordId",
                table: "Users",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscordId",
                table: "Users");
        }
    }
}
