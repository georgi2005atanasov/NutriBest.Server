using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriBest.Server.Data.Migrations
{
    public partial class RemovedRedundantBrandIdIntBrandLogoEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brands_BrandLogo_BrandLogoId",
                table: "Brands");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BrandLogo",
                table: "BrandLogo");

            migrationBuilder.RenameTable(
                name: "BrandLogo",
                newName: "BrandsLogos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BrandsLogos",
                table: "BrandsLogos",
                column: "BrandLogoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Brands_BrandsLogos_BrandLogoId",
                table: "Brands",
                column: "BrandLogoId",
                principalTable: "BrandsLogos",
                principalColumn: "BrandLogoId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brands_BrandsLogos_BrandLogoId",
                table: "Brands");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BrandsLogos",
                table: "BrandsLogos");

            migrationBuilder.RenameTable(
                name: "BrandsLogos",
                newName: "BrandLogo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BrandLogo",
                table: "BrandLogo",
                column: "BrandLogoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Brands_BrandLogo_BrandLogoId",
                table: "Brands",
                column: "BrandLogoId",
                principalTable: "BrandLogo",
                principalColumn: "BrandLogoId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
