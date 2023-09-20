using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrantBot.Data.Migrations
{
    public partial class AddAwardWeight : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Weight",
                table: "Awards",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Awards");
        }
    }
}
