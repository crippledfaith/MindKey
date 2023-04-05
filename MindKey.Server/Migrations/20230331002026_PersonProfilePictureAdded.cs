using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindKey.Server.Migrations
{
    /// <inheritdoc />
    public partial class PersonProfilePictureAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "People",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "People");
        }
    }
}
