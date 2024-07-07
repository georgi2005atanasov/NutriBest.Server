using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriBest.Server.Data.Migrations
{
    public partial class RemovePostalCodeFromTown : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Cities");

            migrationBuilder.AddColumn<int>(
                name: "PostalCode",
                table: "Addresses",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Addresses");

            migrationBuilder.AddColumn<int>(
                name: "PostalCode",
                table: "Cities",
                type: "int",
                nullable: true);
        }
    }
}
