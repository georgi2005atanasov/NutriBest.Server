using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriBest.Server.Data.Migrations
{
    public partial class MadeOneToManyBetweenShippingAndCountry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingDiscounts_Countries_CountryId",
                table: "ShippingDiscounts");

            migrationBuilder.DropIndex(
                name: "IX_ShippingDiscounts_CountryId",
                table: "ShippingDiscounts");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "ShippingDiscounts");

            migrationBuilder.AddColumn<int>(
                name: "ShippingDiscountId",
                table: "Countries",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_ShippingDiscountId",
                table: "Countries",
                column: "ShippingDiscountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Countries_ShippingDiscounts_ShippingDiscountId",
                table: "Countries",
                column: "ShippingDiscountId",
                principalTable: "ShippingDiscounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Countries_ShippingDiscounts_ShippingDiscountId",
                table: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Countries_ShippingDiscountId",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "ShippingDiscountId",
                table: "Countries");

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "ShippingDiscounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingDiscounts_CountryId",
                table: "ShippingDiscounts",
                column: "CountryId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingDiscounts_Countries_CountryId",
                table: "ShippingDiscounts",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
