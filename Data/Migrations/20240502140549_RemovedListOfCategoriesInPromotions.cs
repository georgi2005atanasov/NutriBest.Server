using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriBest.Server.Data.Migrations
{
    public partial class RemovedListOfCategoriesInPromotions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Promotions_PromotionId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_PromotionId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "PromotionId",
                table: "Categories");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Promotions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Promotions");

            migrationBuilder.AddColumn<int>(
                name: "PromotionId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_PromotionId",
                table: "Categories",
                column: "PromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Promotions_PromotionId",
                table: "Categories",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "PromotionId");
        }
    }
}
