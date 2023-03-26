using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindKey.Server.Migrations
{
    /// <inheritdoc />
    public partial class Idea_IsPublished : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Ideas",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Ideas");
        }
    }
}
