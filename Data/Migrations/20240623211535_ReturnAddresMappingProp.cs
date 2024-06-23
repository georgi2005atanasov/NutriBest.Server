using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriBest.Server.Data.Migrations
{
    public partial class ReturnAddresMappingProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileUserId",
                table: "Addresses",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ProfileUserId",
                table: "Addresses",
                column: "ProfileUserId",
                unique: true,
                filter: "[ProfileUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Profiles_ProfileUserId",
                table: "Addresses",
                column: "ProfileUserId",
                principalTable: "Profiles",
                principalColumn: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Profiles_ProfileUserId",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_ProfileUserId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "ProfileUserId",
                table: "Addresses");
        }
    }
}
