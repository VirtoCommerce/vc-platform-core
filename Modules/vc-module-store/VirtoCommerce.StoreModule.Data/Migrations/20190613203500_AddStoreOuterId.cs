using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.StoreModule.Data.Migrations
{
    public partial class AddStoreOuterId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "StoreSeoInfo",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "Store",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "StoreSeoInfo");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "Store");
        }
    }
}
