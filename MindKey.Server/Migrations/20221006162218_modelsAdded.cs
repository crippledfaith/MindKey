using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindKey.Server.Migrations
{
    public partial class modelsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdeaUserComments_People_PersonId",
                table: "IdeaUserComments");

            migrationBuilder.RenameColumn(
                name: "PersonId",
                table: "IdeaUserComments",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "IdeaUserComments",
                newName: "Title");

            migrationBuilder.RenameIndex(
                name: "IX_IdeaUserComments_PersonId",
                table: "IdeaUserComments",
                newName: "IX_IdeaUserComments_UserId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "IdeaUserComments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "IdeaId",
                table: "IdeaUserComments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AgainstCount",
                table: "Ideas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ForCount",
                table: "Ideas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "NetrulCount",
                table: "Ideas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "PostDateTime",
                table: "Ideas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_IdeaUserComments_IdeaId",
                table: "IdeaUserComments",
                column: "IdeaId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdeaUserComments_Ideas_IdeaId",
                table: "IdeaUserComments",
                column: "IdeaId",
                principalTable: "Ideas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IdeaUserComments_Users_UserId",
                table: "IdeaUserComments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdeaUserComments_Ideas_IdeaId",
                table: "IdeaUserComments");

            migrationBuilder.DropForeignKey(
                name: "FK_IdeaUserComments_Users_UserId",
                table: "IdeaUserComments");

            migrationBuilder.DropIndex(
                name: "IX_IdeaUserComments_IdeaId",
                table: "IdeaUserComments");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "IdeaUserComments");

            migrationBuilder.DropColumn(
                name: "IdeaId",
                table: "IdeaUserComments");

            migrationBuilder.DropColumn(
                name: "AgainstCount",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "ForCount",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "NetrulCount",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "PostDateTime",
                table: "Ideas");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "IdeaUserComments",
                newName: "PersonId");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "IdeaUserComments",
                newName: "Comment");

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
    }
}
