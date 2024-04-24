using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriBest.Server.Data.Migrations
{
    public partial class AddedAuditInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                defaultValue: null,
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
