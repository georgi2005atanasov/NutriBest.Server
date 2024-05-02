using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriBest.Server.Data.Migrations
{
    public partial class RemovedPromotionIdColumnToCategoriesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Promotions_PromotionId",
                table: "Categories");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Promotions_PromotionId",
                table: "Categories",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "PromotionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Promotions_PromotionId",
                table: "Categories");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Promotions_PromotionId",
                table: "Categories",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "PromotionId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
