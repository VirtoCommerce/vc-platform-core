using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.InventoryModule.Data.Migrations
{
    public partial class AddInventorySupportSoftDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Inventory",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FulfillmentCenter",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FulfillmentCenter");
        }
    }
}
