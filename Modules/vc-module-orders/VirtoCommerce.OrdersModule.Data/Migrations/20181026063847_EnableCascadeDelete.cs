using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.OrdersModule.Data.Migrations
{
    public partial class EnableCascadeDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderAddress_CustomerOrder_CustomerOrderId",
                table: "OrderAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderAddress_OrderPaymentIn_PaymentInId",
                table: "OrderAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderAddress_OrderShipment_ShipmentId",
                table: "OrderAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDiscount_CustomerOrder_CustomerOrderId",
                table: "OrderDiscount");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDiscount_OrderLineItem_LineItemId",
                table: "OrderDiscount");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDiscount_OrderPaymentIn_PaymentInId",
                table: "OrderDiscount");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDiscount_OrderShipment_ShipmentId",
                table: "OrderDiscount");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderShipmentItem_OrderShipmentPackage_ShipmentPackageId",
                table: "OrderShipmentItem");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderShipmentPackage_OrderShipment_ShipmentId",
                table: "OrderShipmentPackage");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderTaxDetail_CustomerOrder_CustomerOrderId",
                table: "OrderTaxDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderTaxDetail_OrderLineItem_LineItemId",
                table: "OrderTaxDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderTaxDetail_OrderPaymentIn_PaymentInId",
                table: "OrderTaxDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderTaxDetail_OrderShipment_ShipmentId",
                table: "OrderTaxDetail");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAddress_CustomerOrder_CustomerOrderId",
                table: "OrderAddress",
                column: "CustomerOrderId",
                principalTable: "CustomerOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAddress_OrderPaymentIn_PaymentInId",
                table: "OrderAddress",
                column: "PaymentInId",
                principalTable: "OrderPaymentIn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAddress_OrderShipment_ShipmentId",
                table: "OrderAddress",
                column: "ShipmentId",
                principalTable: "OrderShipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDiscount_CustomerOrder_CustomerOrderId",
                table: "OrderDiscount",
                column: "CustomerOrderId",
                principalTable: "CustomerOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDiscount_OrderLineItem_LineItemId",
                table: "OrderDiscount",
                column: "LineItemId",
                principalTable: "OrderLineItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDiscount_OrderPaymentIn_PaymentInId",
                table: "OrderDiscount",
                column: "PaymentInId",
                principalTable: "OrderPaymentIn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDiscount_OrderShipment_ShipmentId",
                table: "OrderDiscount",
                column: "ShipmentId",
                principalTable: "OrderShipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderShipmentItem_OrderShipmentPackage_ShipmentPackageId",
                table: "OrderShipmentItem",
                column: "ShipmentPackageId",
                principalTable: "OrderShipmentPackage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderShipmentPackage_OrderShipment_ShipmentId",
                table: "OrderShipmentPackage",
                column: "ShipmentId",
                principalTable: "OrderShipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTaxDetail_CustomerOrder_CustomerOrderId",
                table: "OrderTaxDetail",
                column: "CustomerOrderId",
                principalTable: "CustomerOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTaxDetail_OrderLineItem_LineItemId",
                table: "OrderTaxDetail",
                column: "LineItemId",
                principalTable: "OrderLineItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTaxDetail_OrderPaymentIn_PaymentInId",
                table: "OrderTaxDetail",
                column: "PaymentInId",
                principalTable: "OrderPaymentIn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTaxDetail_OrderShipment_ShipmentId",
                table: "OrderTaxDetail",
                column: "ShipmentId",
                principalTable: "OrderShipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderAddress_CustomerOrder_CustomerOrderId",
                table: "OrderAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderAddress_OrderPaymentIn_PaymentInId",
                table: "OrderAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderAddress_OrderShipment_ShipmentId",
                table: "OrderAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDiscount_CustomerOrder_CustomerOrderId",
                table: "OrderDiscount");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDiscount_OrderLineItem_LineItemId",
                table: "OrderDiscount");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDiscount_OrderPaymentIn_PaymentInId",
                table: "OrderDiscount");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDiscount_OrderShipment_ShipmentId",
                table: "OrderDiscount");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderShipmentItem_OrderShipmentPackage_ShipmentPackageId",
                table: "OrderShipmentItem");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderShipmentPackage_OrderShipment_ShipmentId",
                table: "OrderShipmentPackage");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderTaxDetail_CustomerOrder_CustomerOrderId",
                table: "OrderTaxDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderTaxDetail_OrderLineItem_LineItemId",
                table: "OrderTaxDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderTaxDetail_OrderPaymentIn_PaymentInId",
                table: "OrderTaxDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderTaxDetail_OrderShipment_ShipmentId",
                table: "OrderTaxDetail");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAddress_CustomerOrder_CustomerOrderId",
                table: "OrderAddress",
                column: "CustomerOrderId",
                principalTable: "CustomerOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAddress_OrderPaymentIn_PaymentInId",
                table: "OrderAddress",
                column: "PaymentInId",
                principalTable: "OrderPaymentIn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAddress_OrderShipment_ShipmentId",
                table: "OrderAddress",
                column: "ShipmentId",
                principalTable: "OrderShipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDiscount_CustomerOrder_CustomerOrderId",
                table: "OrderDiscount",
                column: "CustomerOrderId",
                principalTable: "CustomerOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDiscount_OrderLineItem_LineItemId",
                table: "OrderDiscount",
                column: "LineItemId",
                principalTable: "OrderLineItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDiscount_OrderPaymentIn_PaymentInId",
                table: "OrderDiscount",
                column: "PaymentInId",
                principalTable: "OrderPaymentIn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDiscount_OrderShipment_ShipmentId",
                table: "OrderDiscount",
                column: "ShipmentId",
                principalTable: "OrderShipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderShipmentItem_OrderShipmentPackage_ShipmentPackageId",
                table: "OrderShipmentItem",
                column: "ShipmentPackageId",
                principalTable: "OrderShipmentPackage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderShipmentPackage_OrderShipment_ShipmentId",
                table: "OrderShipmentPackage",
                column: "ShipmentId",
                principalTable: "OrderShipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTaxDetail_CustomerOrder_CustomerOrderId",
                table: "OrderTaxDetail",
                column: "CustomerOrderId",
                principalTable: "CustomerOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTaxDetail_OrderLineItem_LineItemId",
                table: "OrderTaxDetail",
                column: "LineItemId",
                principalTable: "OrderLineItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTaxDetail_OrderPaymentIn_PaymentInId",
                table: "OrderTaxDetail",
                column: "PaymentInId",
                principalTable: "OrderPaymentIn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTaxDetail_OrderShipment_ShipmentId",
                table: "OrderTaxDetail",
                column: "ShipmentId",
                principalTable: "OrderShipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
