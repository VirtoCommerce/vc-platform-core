using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.CartModule.Data.Migrations
{
    public partial class AddCartDynamicPropertyObjectValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartDynamicPropertyObjectValue",
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
                    ShoppingCartId = table.Column<string>(nullable: true),
                    ShipmentId = table.Column<string>(nullable: true),
                    PaymentId = table.Column<string>(nullable: true),
                    LineItemId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartDynamicPropertyObjectValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartDynamicPropertyObjectValue_CartLineItem_LineItemId",
                        column: x => x.LineItemId,
                        principalTable: "CartLineItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartDynamicPropertyObjectValue_CartPayment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "CartPayment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartDynamicPropertyObjectValue_CartShipment_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "CartShipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartDynamicPropertyObjectValue_Cart_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "Cart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartDynamicPropertyObjectValue_LineItemId",
                table: "CartDynamicPropertyObjectValue",
                column: "LineItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CartDynamicPropertyObjectValue_PaymentId",
                table: "CartDynamicPropertyObjectValue",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_CartDynamicPropertyObjectValue_ShipmentId",
                table: "CartDynamicPropertyObjectValue",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CartDynamicPropertyObjectValue_ShoppingCartId",
                table: "CartDynamicPropertyObjectValue",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectType_LineItemId",
                table: "CartDynamicPropertyObjectValue",
                columns: new[] { "ObjectType", "LineItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectType_ObjectId",
                table: "CartDynamicPropertyObjectValue",
                columns: new[] { "ObjectType", "ObjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectType_PaymentId",
                table: "CartDynamicPropertyObjectValue",
                columns: new[] { "ObjectType", "PaymentId" });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectType_ShipmentId",
                table: "CartDynamicPropertyObjectValue",
                columns: new[] { "ObjectType", "ShipmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectType_ShoppingCartId",
                table: "CartDynamicPropertyObjectValue",
                columns: new[] { "ObjectType", "ShoppingCartId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartDynamicPropertyObjectValue");
        }
    }
}
