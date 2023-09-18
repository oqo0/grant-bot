using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrantBot.Data.Migrations
{
    public partial class AddAwardUniqueId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UniqueId",
                table: "Awards",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UniqueId",
                table: "Awards");
        }
    }
}
