using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repo.Data.Migrations
{
    public partial class EditNewChatModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_AppUserId",
                table: "Friendships");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "Friendships",
                newName: "User2Id");

            migrationBuilder.RenameIndex(
                name: "IX_Friendships_AppUserId",
                table: "Friendships",
                newName: "IX_Friendships_User2Id");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Friendships",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "User1Id",
                table: "Friendships",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_User1Id",
                table: "Friendships",
                column: "User1Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_User1Id",
                table: "Friendships",
                column: "User1Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_User2Id",
                table: "Friendships",
                column: "User2Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_User1Id",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_User2Id",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_User1Id",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "User1Id",
                table: "Friendships");

            migrationBuilder.RenameColumn(
                name: "User2Id",
                table: "Friendships",
                newName: "AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Friendships_User2Id",
                table: "Friendships",
                newName: "IX_Friendships_AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_AppUserId",
                table: "Friendships",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
