using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriBest.Server.Data.Migrations
{
    public partial class RenamedNewsetters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Newsletters",
                table: "Newsletters");

            migrationBuilder.RenameTable(
                name: "Newsletters",
                newName: "Newsletter");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Newsletter",
                table: "Newsletter",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Newsletter",
                table: "Newsletter");

            migrationBuilder.RenameTable(
                name: "Newsletter",
                newName: "Newsletters");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Newsletters",
                table: "Newsletters",
                column: "Id");
        }
    }
}
