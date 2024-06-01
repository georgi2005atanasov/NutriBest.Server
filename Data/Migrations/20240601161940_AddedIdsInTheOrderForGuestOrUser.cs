using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriBest.Server.Data.Migrations
{
    public partial class AddedIdsInTheOrderForGuestOrUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuestsOrders_Orders_OrderId",
                table: "GuestsOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersOrders_Orders_OrderId",
                table: "UsersOrders");

            migrationBuilder.DropIndex(
                name: "IX_UsersOrders_OrderId",
                table: "UsersOrders");

            migrationBuilder.DropIndex(
                name: "IX_GuestsOrders_OrderId",
                table: "GuestsOrders");

            migrationBuilder.AddColumn<int>(
                name: "GuestOrderId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GuestOrderId1",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserOrderId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserOrderId1",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_GuestOrderId1",
                table: "Orders",
                column: "GuestOrderId1");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserOrderId1",
                table: "Orders",
                column: "UserOrderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_GuestsOrders_GuestOrderId1",
                table: "Orders",
                column: "GuestOrderId1",
                principalTable: "GuestsOrders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_UsersOrders_UserOrderId1",
                table: "Orders",
                column: "UserOrderId1",
                principalTable: "UsersOrders",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_GuestsOrders_GuestOrderId1",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_UsersOrders_UserOrderId1",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_GuestOrderId1",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_UserOrderId1",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GuestOrderId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GuestOrderId1",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UserOrderId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UserOrderId1",
                table: "Orders");

            migrationBuilder.CreateIndex(
                name: "IX_UsersOrders_OrderId",
                table: "UsersOrders",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GuestsOrders_OrderId",
                table: "GuestsOrders",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GuestsOrders_Orders_OrderId",
                table: "GuestsOrders",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersOrders_Orders_OrderId",
                table: "UsersOrders",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
