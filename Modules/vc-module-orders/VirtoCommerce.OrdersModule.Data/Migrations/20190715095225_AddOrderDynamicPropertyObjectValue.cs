using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.OrdersModule.Data.Migrations
{
    public partial class AddOrderDynamicPropertyObjectValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderDynamicPropertyObjectValue",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ObjectType = table.Column<string>(maxLength: 256, nullable: true),
                    ObjectId = table.Column<string>(maxLength: 128, nullable: true),
                    Locale = table.Column<string>(maxLength: 64, nullable: true),
                    ValueType = table.Column<string>(maxLength: 64, nullable: false),
                    ShortTextValue = table.Column<string>(maxLength: 512, nullable: true),
                    LongTextValue = table.Column<string>(nullable: true),
                    DecimalValue = table.Column<decimal>(type: "decimal(18,5)", nullable: true),
                    IntegerValue = table.Column<int>(nullable: true),
                    BooleanValue = table.Column<bool>(nullable: true),
                    DateTimeValue = table.Column<DateTime>(nullable: true),
                    PropertyId = table.Column<string>(maxLength: 128, nullable: true),
                    DictionaryItemId = table.Column<string>(maxLength: 128, nullable: true),
                    PropertyName = table.Column<string>(maxLength: 256, nullable: true),
                    CustomerOrderId = table.Column<string>(nullable: true),
                    PaymentInId = table.Column<string>(nullable: true),
                    ShipmentId = table.Column<string>(nullable: true),
                    LineItemId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDynamicPropertyObjectValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDynamicPropertyObjectValue_CustomerOrder_CustomerOrderId",
                        column: x => x.CustomerOrderId,
                        principalTable: "CustomerOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDynamicPropertyObjectValue_OrderLineItem_LineItemId",
                        column: x => x.LineItemId,
                        principalTable: "OrderLineItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDynamicPropertyObjectValue_OrderPaymentIn_PaymentInId",
                        column: x => x.PaymentInId,
                        principalTable: "OrderPaymentIn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDynamicPropertyObjectValue_OrderShipment_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "OrderShipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDynamicPropertyObjectValue_CustomerOrderId",
                table: "OrderDynamicPropertyObjectValue",
                column: "CustomerOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDynamicPropertyObjectValue_LineItemId",
                table: "OrderDynamicPropertyObjectValue",
                column: "LineItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDynamicPropertyObjectValue_PaymentInId",
                table: "OrderDynamicPropertyObjectValue",
                column: "PaymentInId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDynamicPropertyObjectValue_ShipmentId",
                table: "OrderDynamicPropertyObjectValue",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectType_CustomerOrderId",
                table: "OrderDynamicPropertyObjectValue",
                columns: new[] { "ObjectType", "CustomerOrderId" });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectType_LineItemId",
                table: "OrderDynamicPropertyObjectValue",
                columns: new[] { "ObjectType", "LineItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectType_ObjectId",
                table: "OrderDynamicPropertyObjectValue",
                columns: new[] { "ObjectType", "ObjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectType_PaymentInId",
                table: "OrderDynamicPropertyObjectValue",
                columns: new[] { "ObjectType", "PaymentInId" });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectType_ShipmentId",
                table: "OrderDynamicPropertyObjectValue",
                columns: new[] { "ObjectType", "ShipmentId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDynamicPropertyObjectValue");
        }
    }
}
