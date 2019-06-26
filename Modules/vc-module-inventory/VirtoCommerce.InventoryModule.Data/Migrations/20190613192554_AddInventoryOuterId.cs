using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.InventoryModule.Data.Migrations
{
    public partial class AddInventoryOuterId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "Inventory",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "FulfillmentCenter",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "FulfillmentCenter");
        }
    }
}
