using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriBest.Server.Data.Migrations
{
    public partial class RemovedGuestTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GuestId",
                table: "GuestsOrders",
                newName: "Id");

            migrationBuilder.AddColumn<DateTime>(
                name: "MadeOn",
                table: "OrdersDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MadeOn",
                table: "OrdersDetails");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "GuestsOrders",
                newName: "GuestId");
        }
    }
}
