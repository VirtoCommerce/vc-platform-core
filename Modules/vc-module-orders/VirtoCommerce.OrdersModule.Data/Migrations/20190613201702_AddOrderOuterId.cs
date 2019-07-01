using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.OrdersModule.Data.Migrations
{
    public partial class AddOrderOuterId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "OrderShipmentItem",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "OrderShipment",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "OrderLineItem",
                maxLength: 128,
                nullable: true);

            migrationBuilder.Sql(@"IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '__MigrationHistory'))
                    BEGIN
                         ALTER TABLE [CustomerOrder] ADD [OuterId] nvarchar(128) NULL
                    END");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "OrderShipmentItem");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "OrderShipment");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "OrderLineItem");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "CustomerOrder");
        }
    }
}
