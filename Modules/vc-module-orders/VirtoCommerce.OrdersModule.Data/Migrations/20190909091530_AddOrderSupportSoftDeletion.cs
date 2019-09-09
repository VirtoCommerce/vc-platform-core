using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.OrdersModule.Data.Migrations
{
    public partial class AddOrderSupportSoftDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OrderShipmentItem",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OrderShipment",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OrderPaymentIn",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OrderLineItem",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CustomerOrder",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OrderShipmentItem");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OrderShipment");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OrderPaymentIn");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OrderLineItem");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CustomerOrder");
        }
    }
}
