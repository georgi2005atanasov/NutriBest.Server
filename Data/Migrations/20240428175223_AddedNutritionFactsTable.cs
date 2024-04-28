using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriBest.Server.Data.Migrations
{
    public partial class AddedNutritionFactsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NutritionFacts",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    EnergyValue = table.Column<double>(type: "float", nullable: true),
                    Fats = table.Column<double>(type: "float", nullable: true),
                    SaturatedFats = table.Column<double>(type: "float", nullable: true),
                    Carbohydrates = table.Column<double>(type: "float", nullable: true),
                    Sugars = table.Column<double>(type: "float", nullable: true),
                    Proteins = table.Column<double>(type: "float", nullable: true),
                    Salt = table.Column<double>(type: "float", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionFacts", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_NutritionFacts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NutritionFacts");
        }
    }
}
