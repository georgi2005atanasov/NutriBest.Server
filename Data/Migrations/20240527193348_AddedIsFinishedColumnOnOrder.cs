using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriBest.Server.Data.Migrations
{
    public partial class AddedIsFinishedColumnOnOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFinished",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFinished",
                table: "Orders");
        }
    }
}
