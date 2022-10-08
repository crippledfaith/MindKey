using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindKey.Server.Migrations
{
    public partial class modalupdate3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionShort",
                table: "Ideas",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionShort",
                table: "Ideas");
        }
    }
}
