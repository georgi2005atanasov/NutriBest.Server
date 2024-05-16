using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriBest.Server.Data.Migrations
{
    public partial class RemovedServingsPerContainerColumnBecauseRedundant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServingsPerContainer",
                table: "ProductsDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServingsPerContainer",
                table: "ProductsDetails",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
