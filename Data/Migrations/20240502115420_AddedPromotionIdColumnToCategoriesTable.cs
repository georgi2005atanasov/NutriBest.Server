using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriBest.Server.Data.Migrations
{
    public partial class AddedPromotionIdColumnToCategoriesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MinimumPrice",
                table: "Promotions",
                type: "decimal(18,2)",
                nullable: true);

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
                principalColumn: "PromotionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Promotions_PromotionId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_PromotionId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "MinimumPrice",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "PromotionId",
                table: "Categories");
        }
    }
}
