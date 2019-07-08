using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.CartModule.Data.Migrations
{
    public partial class InitialCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 64, nullable: true),
                    StoreId = table.Column<string>(maxLength: 64, nullable: false),
                    ChannelId = table.Column<string>(maxLength: 64, nullable: true),
                    IsAnonymous = table.Column<bool>(nullable: false),
                    CustomerId = table.Column<string>(maxLength: 64, nullable: false),
                    CustomerName = table.Column<string>(maxLength: 128, nullable: true),
                    OrganizationId = table.Column<string>(maxLength: 64, nullable: true),
                    Currency = table.Column<string>(maxLength: 3, nullable: false),
                    LanguageCode = table.Column<string>(maxLength: 16, nullable: true),
                    TaxIncluded = table.Column<bool>(nullable: false),
                    IsRecuring = table.Column<bool>(nullable: false),
                    Comment = table.Column<string>(maxLength: 2048, nullable: true),
                    Total = table.Column<decimal>(type: "Money", nullable: false),
                    SubTotal = table.Column<decimal>(type: "Money", nullable: false),
                    SubTotalWithTax = table.Column<decimal>(type: "Money", nullable: false),
                    ShippingTotal = table.Column<decimal>(type: "Money", nullable: false),
                    ShippingTotalWithTax = table.Column<decimal>(type: "Money", nullable: false),
                    PaymentTotal = table.Column<decimal>(type: "Money", nullable: false),
                    PaymentTotalWithTax = table.Column<decimal>(type: "Money", nullable: false),
                    HandlingTotal = table.Column<decimal>(type: "Money", nullable: false),
                    HandlingTotalWithTax = table.Column<decimal>(type: "Money", nullable: false),
                    DiscountTotal = table.Column<decimal>(type: "Money", nullable: false),
                    DiscountTotalWithTax = table.Column<decimal>(type: "Money", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "Money", nullable: false),
                    TaxTotal = table.Column<decimal>(type: "Money", nullable: false),
                    ValidationType = table.Column<string>(maxLength: 64, nullable: true),
                    Status = table.Column<string>(maxLength: 64, nullable: true),
                    Fee = table.Column<decimal>(type: "Money", nullable: false),
                    FeeWithTax = table.Column<decimal>(type: "Money", nullable: false),
                    TaxPercentRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Type = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cart", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartCoupon",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Code = table.Column<string>(maxLength: 64, nullable: true),
                    ShoppingCartId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartCoupon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartCoupon_Cart_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "Cart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartLineItem",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Currency = table.Column<string>(maxLength: 3, nullable: false),
                    ProductId = table.Column<string>(maxLength: 64, nullable: false),
                    Sku = table.Column<string>(maxLength: 64, nullable: false),
                    CatalogId = table.Column<string>(maxLength: 64, nullable: false),
                    CategoryId = table.Column<string>(maxLength: 64, nullable: true),
                    ProductType = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    FulfillmentLocationCode = table.Column<string>(maxLength: 64, nullable: true),
                    ShipmentMethodCode = table.Column<string>(maxLength: 64, nullable: true),
                    RequiredShipping = table.Column<bool>(nullable: false),
                    ImageUrl = table.Column<string>(maxLength: 1028, nullable: true),
                    IsGift = table.Column<bool>(nullable: false),
                    LanguageCode = table.Column<string>(maxLength: 16, nullable: true),
                    Comment = table.Column<string>(maxLength: 2048, nullable: true),
                    ValidationType = table.Column<string>(maxLength: 64, nullable: true),
                    IsReccuring = table.Column<bool>(nullable: false),
                    TaxIncluded = table.Column<bool>(nullable: false),
                    VolumetricWeight = table.Column<decimal>(nullable: true),
                    WeightUnit = table.Column<string>(maxLength: 32, nullable: true),
                    Weight = table.Column<decimal>(nullable: true),
                    MeasureUnit = table.Column<string>(maxLength: 32, nullable: true),
                    Height = table.Column<decimal>(nullable: true),
                    Length = table.Column<decimal>(nullable: true),
                    Width = table.Column<decimal>(nullable: true),
                    IsReadOnly = table.Column<bool>(nullable: false),
                    PriceId = table.Column<string>(maxLength: 128, nullable: true),
                    ListPrice = table.Column<decimal>(type: "Money", nullable: false),
                    ListPriceWithTax = table.Column<decimal>(type: "Money", nullable: false),
                    SalePrice = table.Column<decimal>(type: "Money", nullable: false),
                    SalePriceWithTax = table.Column<decimal>(type: "Money", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "Money", nullable: false),
                    DiscountAmountWithTax = table.Column<decimal>(type: "Money", nullable: false),
                    Fee = table.Column<decimal>(type: "Money", nullable: false),
                    FeeWithTax = table.Column<decimal>(type: "Money", nullable: false),
                    TaxTotal = table.Column<decimal>(type: "Money", nullable: false),
                    TaxPercentRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TaxType = table.Column<string>(maxLength: 64, nullable: true),
                    ShoppingCartId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartLineItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartLineItem_Cart_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "Cart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartPayment",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Currency = table.Column<string>(maxLength: 64, nullable: false),
                    PaymentGatewayCode = table.Column<string>(maxLength: 64, nullable: true),
                    Amount = table.Column<decimal>(type: "Money", nullable: false),
                    Purpose = table.Column<string>(maxLength: 1024, nullable: true),
                    TaxType = table.Column<string>(maxLength: 64, nullable: true),
                    Price = table.Column<decimal>(type: "Money", nullable: false),
                    PriceWithTax = table.Column<decimal>(type: "Money", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "Money", nullable: false),
                    DiscountAmountWithTax = table.Column<decimal>(type: "Money", nullable: false),
                    Total = table.Column<decimal>(type: "Money", nullable: false),
                    TotalWithTax = table.Column<decimal>(type: "Money", nullable: false),
                    TaxTotal = table.Column<decimal>(type: "Money", nullable: false),
                    TaxPercentRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ShoppingCartId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartPayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartPayment_Cart_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "Cart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartShipment",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ShipmentMethodCode = table.Column<string>(maxLength: 64, nullable: true),
                    ShipmentMethodOption = table.Column<string>(maxLength: 64, nullable: true),
                    FulfillmentCenterId = table.Column<string>(maxLength: 64, nullable: true),
                    Currency = table.Column<string>(maxLength: 3, nullable: false),
                    WeightUnit = table.Column<string>(maxLength: 16, nullable: true),
                    WeightValue = table.Column<decimal>(nullable: true),
                    VolumetricWeight = table.Column<decimal>(nullable: true),
                    DimensionUnit = table.Column<string>(maxLength: 16, nullable: true),
                    DimensionHeight = table.Column<decimal>(nullable: true),
                    DimensionLength = table.Column<decimal>(nullable: true),
                    DimensionWidth = table.Column<decimal>(nullable: true),
                    TaxIncluded = table.Column<bool>(nullable: false),
                    Price = table.Column<decimal>(type: "Money", nullable: false),
                    PriceWithTax = table.Column<decimal>(type: "Money", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "Money", nullable: false),
                    DiscountAmountWithTax = table.Column<decimal>(type: "Money", nullable: false),
                    TaxPercentRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TaxTotal = table.Column<decimal>(type: "Money", nullable: false),
                    Total = table.Column<decimal>(type: "Money", nullable: false),
                    TotalWithTax = table.Column<decimal>(type: "Money", nullable: false),
                    Fee = table.Column<decimal>(type: "Money", nullable: false),
                    FeeWithTax = table.Column<decimal>(type: "Money", nullable: false),
                    TaxType = table.Column<string>(maxLength: 64, nullable: true),
                    ShoppingCartId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartShipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartShipment_Cart_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "Cart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartAddress",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 2048, nullable: true),
                    AddressType = table.Column<string>(maxLength: 32, nullable: true),
                    Organization = table.Column<string>(maxLength: 64, nullable: true),
                    CountryCode = table.Column<string>(maxLength: 3, nullable: true),
                    CountryName = table.Column<string>(maxLength: 64, nullable: false),
                    City = table.Column<string>(maxLength: 128, nullable: false),
                    PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    Line1 = table.Column<string>(maxLength: 2048, nullable: true),
                    Line2 = table.Column<string>(maxLength: 2048, nullable: true),
                    RegionId = table.Column<string>(maxLength: 128, nullable: true),
                    RegionName = table.Column<string>(maxLength: 128, nullable: true),
                    FirstName = table.Column<string>(maxLength: 64, nullable: false),
                    LastName = table.Column<string>(maxLength: 64, nullable: false),
                    Phone = table.Column<string>(maxLength: 64, nullable: true),
                    Email = table.Column<string>(maxLength: 254, nullable: true),
                    ShoppingCartId = table.Column<string>(nullable: true),
                    ShipmentId = table.Column<string>(nullable: true),
                    PaymentId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartAddress_CartPayment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "CartPayment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartAddress_CartShipment_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "CartShipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartAddress_Cart_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "Cart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CartDiscount",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    PromotionId = table.Column<string>(maxLength: 64, nullable: true),
                    PromotionDescription = table.Column<string>(maxLength: 1024, nullable: true),
                    CouponCode = table.Column<string>(maxLength: 64, nullable: true),
                    Currency = table.Column<string>(maxLength: 3, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "Money", nullable: false),
                    DiscountAmountWithTax = table.Column<decimal>(type: "Money", nullable: false),
                    ShoppingCartId = table.Column<string>(nullable: true),
                    ShipmentId = table.Column<string>(nullable: true),
                    LineItemId = table.Column<string>(nullable: true),
                    PaymentId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartDiscount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartDiscount_CartLineItem_LineItemId",
                        column: x => x.LineItemId,
                        principalTable: "CartLineItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartDiscount_CartPayment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "CartPayment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartDiscount_CartShipment_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "CartShipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartDiscount_Cart_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "Cart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CartShipmentItem",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    BarCode = table.Column<string>(maxLength: 128, nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    LineItemId = table.Column<string>(nullable: false),
                    ShipmentId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartShipmentItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartShipmentItem_CartLineItem_LineItemId",
                        column: x => x.LineItemId,
                        principalTable: "CartLineItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartShipmentItem_CartShipment_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "CartShipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CartTaxDetail",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 1024, nullable: true),
                    Rate = table.Column<decimal>(nullable: false),
                    Amount = table.Column<decimal>(type: "Money", nullable: false),
                    ShoppingCartId = table.Column<string>(nullable: true),
                    ShipmentId = table.Column<string>(nullable: true),
                    LineItemId = table.Column<string>(nullable: true),
                    PaymentId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartTaxDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartTaxDetail_CartLineItem_LineItemId",
                        column: x => x.LineItemId,
                        principalTable: "CartLineItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartTaxDetail_CartPayment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "CartPayment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartTaxDetail_CartShipment_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "CartShipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartTaxDetail_Cart_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "Cart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartAddress_PaymentId",
                table: "CartAddress",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_CartAddress_ShipmentId",
                table: "CartAddress",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CartAddress_ShoppingCartId",
                table: "CartAddress",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartCoupon_ShoppingCartId",
                table: "CartCoupon",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartDiscount_LineItemId",
                table: "CartDiscount",
                column: "LineItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CartDiscount_PaymentId",
                table: "CartDiscount",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_CartDiscount_ShipmentId",
                table: "CartDiscount",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CartDiscount_ShoppingCartId",
                table: "CartDiscount",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartLineItem_ShoppingCartId",
                table: "CartLineItem",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartPayment_ShoppingCartId",
                table: "CartPayment",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartShipment_ShoppingCartId",
                table: "CartShipment",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartShipmentItem_LineItemId",
                table: "CartShipmentItem",
                column: "LineItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CartShipmentItem_ShipmentId",
                table: "CartShipmentItem",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CartTaxDetail_LineItemId",
                table: "CartTaxDetail",
                column: "LineItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CartTaxDetail_PaymentId",
                table: "CartTaxDetail",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_CartTaxDetail_ShipmentId",
                table: "CartTaxDetail",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CartTaxDetail_ShoppingCartId",
                table: "CartTaxDetail",
                column: "ShoppingCartId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartAddress");

            migrationBuilder.DropTable(
                name: "CartCoupon");

            migrationBuilder.DropTable(
                name: "CartDiscount");

            migrationBuilder.DropTable(
                name: "CartShipmentItem");

            migrationBuilder.DropTable(
                name: "CartTaxDetail");

            migrationBuilder.DropTable(
                name: "CartLineItem");

            migrationBuilder.DropTable(
                name: "CartPayment");

            migrationBuilder.DropTable(
                name: "CartShipment");

            migrationBuilder.DropTable(
                name: "Cart");
        }
    }
}
