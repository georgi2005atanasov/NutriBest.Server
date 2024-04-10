using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriBest.Server.Data.Migrations
{
    public partial class AddedProductCategoriesMappingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductsCategories_Categories_CategoriesId",
                table: "ProductsCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductsCategories_Products_ProductsProductId",
                table: "ProductsCategories");

            migrationBuilder.RenameColumn(
                name: "ProductsProductId",
                table: "ProductsCategories",
                newName: "CategoryId");

            migrationBuilder.RenameColumn(
                name: "CategoriesId",
                table: "ProductsCategories",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductsCategories_ProductsProductId",
                table: "ProductsCategories",
                newName: "IX_ProductsCategories_CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductsCategories_Categories_CategoryId",
                table: "ProductsCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductsCategories_Products_ProductId",
                table: "ProductsCategories",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductsCategories_Categories_CategoryId",
                table: "ProductsCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductsCategories_Products_ProductId",
                table: "ProductsCategories");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "ProductsCategories",
                newName: "ProductsProductId");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "ProductsCategories",
                newName: "CategoriesId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductsCategories_CategoryId",
                table: "ProductsCategories",
                newName: "IX_ProductsCategories_ProductsProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductsCategories_Categories_CategoriesId",
                table: "ProductsCategories",
                column: "CategoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductsCategories_Products_ProductsProductId",
                table: "ProductsCategories",
                column: "ProductsProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
