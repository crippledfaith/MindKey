using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindKey.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedIdeaCommant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdeaUserComments_Users_UserId",
                table: "IdeaUserComments");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "IdeaUserComments",
                newName: "PersonId");

            migrationBuilder.RenameIndex(
                name: "IX_IdeaUserComments_UserId",
                table: "IdeaUserComments",
                newName: "IX_IdeaUserComments_PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdeaUserComments_People_PersonId",
                table: "IdeaUserComments",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdeaUserComments_People_PersonId",
                table: "IdeaUserComments");

            migrationBuilder.RenameColumn(
                name: "PersonId",
                table: "IdeaUserComments",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_IdeaUserComments_PersonId",
                table: "IdeaUserComments",
                newName: "IX_IdeaUserComments_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdeaUserComments_Users_UserId",
                table: "IdeaUserComments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
