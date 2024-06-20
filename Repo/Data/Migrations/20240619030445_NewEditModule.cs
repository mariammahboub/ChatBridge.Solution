using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repo.Data.Migrations
{
    public partial class NewEditModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "ChatMessages",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "ChatGroups",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "ChatGroups");
        }
    }
}
