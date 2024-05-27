using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriBest.Server.Data.Migrations
{
    public partial class AddedCountryToAdressesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Countries_OrdersDetails_OrderDetailsId",
                table: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Countries_OrderDetailsId",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "OrderDetailsId",
                table: "Countries");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CountryId",
                table: "Addresses",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Countries_CountryId",
                table: "Addresses",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Countries_CountryId",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_CountryId",
                table: "Addresses");

            migrationBuilder.AddColumn<int>(
                name: "OrderDetailsId",
                table: "Countries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_OrderDetailsId",
                table: "Countries",
                column: "OrderDetailsId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Countries_OrdersDetails_OrderDetailsId",
                table: "Countries",
                column: "OrderDetailsId",
                principalTable: "OrdersDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
