using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriBest.Server.Data.Migrations
{
    public partial class EditedTheEntitiesByMakingThemDeletable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NutritionFacts",
                table: "ProductsDetails",
                newName: "ServingsPerContainer");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Promotions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Promotions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Promotions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Promotions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Promotions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Promotions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Promotions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProductsReviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProductsPromotions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OrdersDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProductsReviews");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProductsPromotions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OrdersDetails");

            migrationBuilder.RenameColumn(
                name: "ServingsPerContainer",
                table: "ProductsDetails",
                newName: "NutritionFacts");
        }
    }
}
