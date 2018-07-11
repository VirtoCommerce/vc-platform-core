using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.OrderModule.Data.Migrations
{
    public partial class InitialOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderOperation",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Number = table.Column<string>(maxLength: 64, nullable: false),
                    IsApproved = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(maxLength: 64, nullable: true),
                    Comment = table.Column<string>(maxLength: 2048, nullable: true),
                    Currency = table.Column<string>(maxLength: 3, nullable: false),
                    Sum = table.Column<decimal>(type: "Money", nullable: false),
                    IsCancelled = table.Column<bool>(nullable: false),
                    CancelledDate = table.Column<DateTime>(nullable: true),
                    CancelReason = table.Column<string>(maxLength: 2048, nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    CustomerId = table.Column<string>(maxLength: 64, nullable: true),
                    CustomerName = table.Column<string>(maxLength: 255, nullable: true),
                    StoreId = table.Column<string>(maxLength: 64, nullable: true),
                    StoreName = table.Column<string>(maxLength: 255, nullable: true),
                    ChannelId = table.Column<string>(maxLength: 64, nullable: true),
                    OrganizationId = table.Column<string>(maxLength: 64, nullable: true),
                    OrganizationName = table.Column<string>(maxLength: 255, nullable: true),
                    EmployeeId = table.Column<string>(maxLength: 64, nullable: true),
                    EmployeeName = table.Column<string>(maxLength: 255, nullable: true),
                    SubscriptionId = table.Column<string>(maxLength: 64, nullable: true),
                    SubscriptionNumber = table.Column<string>(maxLength: 64, nullable: true),
                    IsPrototype = table.Column<bool>(nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "Money", nullable: true),
                    TaxTotal = table.Column<decimal>(type: "Money", nullable: true),
                    Total = table.Column<decimal>(type: "Money", nullable: true),
                    SubTotal = table.Column<decimal>(type: "Money", nullable: true),
                    SubTotalWithTax = table.Column<decimal>(type: "Money", nullable: true),
                    ShippingTotal = table.Column<decimal>(type: "Money", nullable: true),
                    ShippingTotalWithTax = table.Column<decimal>(type: "Money", nullable: true),
                    PaymentTotal = table.Column<decimal>(type: "Money", nullable: true),
                    PaymentTotalWithTax = table.Column<decimal>(type: "Money", nullable: true),
                    HandlingTotal = table.Column<decimal>(type: "Money", nullable: true),
                    HandlingTotalWithTax = table.Column<decimal>(type: "Money", nullable: true),
                    DiscountTotal = table.Column<decimal>(type: "Money", nullable: true),
                    DiscountTotalWithTax = table.Column<decimal>(type: "Money", nullable: true),
                    LanguageCode = table.Column<string>(maxLength: 16, nullable: true),
                    TaxPercentRate = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    ShoppingCartId = table.Column<string>(maxLength: 128, nullable: true),
                    PaymentInEntity_OrganizationId = table.Column<string>(maxLength: 64, nullable: true),
                    PaymentInEntity_OrganizationName = table.Column<string>(maxLength: 255, nullable: true),
                    PaymentInEntity_CustomerId = table.Column<string>(maxLength: 64, nullable: true),
                    PaymentInEntity_CustomerName = table.Column<string>(maxLength: 255, nullable: true),
                    IncomingDate = table.Column<DateTime>(nullable: true),
                    OuterId = table.Column<string>(maxLength: 128, nullable: true),
                    Purpose = table.Column<string>(maxLength: 1024, nullable: true),
                    GatewayCode = table.Column<string>(maxLength: 64, nullable: true),
                    AuthorizedDate = table.Column<DateTime>(nullable: true),
                    CapturedDate = table.Column<DateTime>(nullable: true),
                    VoidedDate = table.Column<DateTime>(nullable: true),
                    TaxType = table.Column<string>(maxLength: 64, nullable: true),
                    Price = table.Column<decimal>(type: "Money", nullable: true),
                    PriceWithTax = table.Column<decimal>(type: "Money", nullable: true),
                    PaymentInEntity_DiscountAmount = table.Column<decimal>(type: "Money", nullable: true),
                    DiscountAmountWithTax = table.Column<decimal>(type: "Money", nullable: true),
                    PaymentInEntity_Total = table.Column<decimal>(type: "Money", nullable: true),
                    TotalWithTax = table.Column<decimal>(type: "Money", nullable: true),
                    PaymentInEntity_TaxTotal = table.Column<decimal>(type: "Money", nullable: true),
                    PaymentInEntity_TaxPercentRate = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    CustomerOrderId = table.Column<string>(nullable: true),
                    ShipmentId = table.Column<string>(nullable: true),
                    ShipmentEntity_OrganizationId = table.Column<string>(maxLength: 64, nullable: true),
                    ShipmentEntity_OrganizationName = table.Column<string>(maxLength: 255, nullable: true),
                    FulfillmentCenterId = table.Column<string>(maxLength: 64, nullable: true),
                    FulfillmentCenterName = table.Column<string>(maxLength: 255, nullable: true),
                    ShipmentEntity_EmployeeId = table.Column<string>(maxLength: 64, nullable: true),
                    ShipmentEntity_EmployeeName = table.Column<string>(maxLength: 255, nullable: true),
                    ShipmentMethodCode = table.Column<string>(maxLength: 64, nullable: true),
                    ShipmentMethodOption = table.Column<string>(maxLength: 64, nullable: true),
                    VolumetricWeight = table.Column<decimal>(nullable: true),
                    WeightUnit = table.Column<string>(maxLength: 32, nullable: true),
                    Weight = table.Column<decimal>(nullable: true),
                    MeasureUnit = table.Column<string>(maxLength: 32, nullable: true),
                    Height = table.Column<decimal>(nullable: true),
                    Length = table.Column<decimal>(nullable: true),
                    Width = table.Column<decimal>(nullable: true),
                    ShipmentEntity_TaxType = table.Column<string>(maxLength: 64, nullable: true),
                    ShipmentEntity_Price = table.Column<decimal>(type: "Money", nullable: true),
                    ShipmentEntity_PriceWithTax = table.Column<decimal>(type: "Money", nullable: true),
                    ShipmentEntity_DiscountAmount = table.Column<decimal>(type: "Money", nullable: true),
                    ShipmentEntity_DiscountAmountWithTax = table.Column<decimal>(type: "Money", nullable: true),
                    ShipmentEntity_Total = table.Column<decimal>(type: "Money", nullable: true),
                    ShipmentEntity_TotalWithTax = table.Column<decimal>(type: "Money", nullable: true),
                    ShipmentEntity_TaxTotal = table.Column<decimal>(type: "Money", nullable: true),
                    ShipmentEntity_TaxPercentRate = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    ShipmentEntity_CustomerOrderId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderOperation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderOperation_OrderOperation_CustomerOrderId",
                        column: x => x.CustomerOrderId,
                        principalTable: "OrderOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOperation_OrderOperation_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "OrderOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderOperation_OrderOperation_ShipmentEntity_CustomerOrderId",
                        column: x => x.ShipmentEntity_CustomerOrderId,
                        principalTable: "OrderOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderAddress",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
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
                    CustomerOrderId = table.Column<string>(nullable: true),
                    ShipmentId = table.Column<string>(nullable: true),
                    PaymentInId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderAddress_OrderOperation_CustomerOrderId",
                        column: x => x.CustomerOrderId,
                        principalTable: "OrderOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderAddress_OrderOperation_PaymentInId",
                        column: x => x.PaymentInId,
                        principalTable: "OrderOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderAddress_OrderOperation_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "OrderOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderLineItem",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    PriceId = table.Column<string>(maxLength: 128, nullable: true),
                    Currency = table.Column<string>(maxLength: 3, nullable: false),
                    Price = table.Column<decimal>(type: "Money", nullable: false),
                    PriceWithTax = table.Column<decimal>(type: "Money", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "Money", nullable: false),
                    DiscountAmountWithTax = table.Column<decimal>(type: "Money", nullable: false),
                    TaxTotal = table.Column<decimal>(type: "Money", nullable: false),
                    TaxPercentRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    ProductId = table.Column<string>(maxLength: 64, nullable: false),
                    CatalogId = table.Column<string>(maxLength: 64, nullable: false),
                    CategoryId = table.Column<string>(maxLength: 64, nullable: true),
                    Sku = table.Column<string>(maxLength: 64, nullable: false),
                    ProductType = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Comment = table.Column<string>(maxLength: 2048, nullable: true),
                    IsReccuring = table.Column<bool>(nullable: false),
                    ImageUrl = table.Column<string>(maxLength: 1028, nullable: true),
                    IsGift = table.Column<bool>(nullable: false),
                    ShippingMethodCode = table.Column<string>(maxLength: 64, nullable: true),
                    FulfilmentLocationCode = table.Column<string>(maxLength: 64, nullable: true),
                    WeightUnit = table.Column<string>(maxLength: 32, nullable: true),
                    Weight = table.Column<decimal>(nullable: true),
                    MeasureUnit = table.Column<string>(maxLength: 32, nullable: true),
                    Height = table.Column<decimal>(nullable: true),
                    Length = table.Column<decimal>(nullable: true),
                    Width = table.Column<decimal>(nullable: true),
                    TaxType = table.Column<string>(maxLength: 64, nullable: true),
                    IsCancelled = table.Column<bool>(nullable: false),
                    CancelledDate = table.Column<DateTime>(nullable: true),
                    CancelReason = table.Column<string>(maxLength: 2048, nullable: true),
                    CustomerOrderId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLineItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderLineItem_OrderOperation_CustomerOrderId",
                        column: x => x.CustomerOrderId,
                        principalTable: "OrderOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderPaymentGatewayTransaction",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Amount = table.Column<decimal>(type: "Money", nullable: false),
                    Currency = table.Column<string>(maxLength: 3, nullable: true),
                    IsProcessed = table.Column<bool>(nullable: false),
                    ProcessedDate = table.Column<DateTime>(nullable: true),
                    ProcessError = table.Column<string>(maxLength: 2048, nullable: true),
                    ProcessAttemptCount = table.Column<int>(nullable: false),
                    RequestData = table.Column<string>(nullable: true),
                    ResponseData = table.Column<string>(nullable: true),
                    ResponseCode = table.Column<string>(maxLength: 64, nullable: true),
                    GatewayIpAddress = table.Column<string>(maxLength: 128, nullable: true),
                    Type = table.Column<string>(maxLength: 64, nullable: true),
                    Status = table.Column<string>(maxLength: 64, nullable: true),
                    Note = table.Column<string>(maxLength: 2048, nullable: true),
                    PaymentInId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPaymentGatewayTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderPaymentGatewayTransaction_OrderOperation_PaymentInId",
                        column: x => x.PaymentInId,
                        principalTable: "OrderOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderShipmentPackage",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    BarCode = table.Column<string>(maxLength: 128, nullable: true),
                    PackageType = table.Column<string>(maxLength: 64, nullable: true),
                    WeightUnit = table.Column<string>(maxLength: 32, nullable: true),
                    Weight = table.Column<decimal>(nullable: true),
                    MeasureUnit = table.Column<string>(maxLength: 32, nullable: true),
                    Height = table.Column<decimal>(nullable: true),
                    Length = table.Column<decimal>(nullable: true),
                    Width = table.Column<decimal>(nullable: true),
                    ShipmentId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderShipmentPackage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderShipmentPackage_OrderOperation_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "OrderOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDiscount",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    PromotionId = table.Column<string>(maxLength: 64, nullable: true),
                    PromotionDescription = table.Column<string>(maxLength: 1024, nullable: true),
                    Currency = table.Column<string>(maxLength: 3, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "Money", nullable: false),
                    DiscountAmountWithTax = table.Column<decimal>(type: "Money", nullable: false),
                    CouponCode = table.Column<string>(maxLength: 64, nullable: true),
                    CouponInvalidDescription = table.Column<string>(maxLength: 1024, nullable: true),
                    CustomerOrderId = table.Column<string>(nullable: true),
                    ShipmentId = table.Column<string>(nullable: true),
                    LineItemId = table.Column<string>(nullable: true),
                    PaymentInId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDiscount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDiscount_OrderOperation_CustomerOrderId",
                        column: x => x.CustomerOrderId,
                        principalTable: "OrderOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDiscount_OrderLineItem_LineItemId",
                        column: x => x.LineItemId,
                        principalTable: "OrderLineItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDiscount_OrderOperation_PaymentInId",
                        column: x => x.PaymentInId,
                        principalTable: "OrderOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDiscount_OrderOperation_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "OrderOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderTaxDetail",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Rate = table.Column<decimal>(nullable: false),
                    Amount = table.Column<decimal>(type: "Money", nullable: false),
                    Name = table.Column<string>(maxLength: 1024, nullable: true),
                    CustomerOrderId = table.Column<string>(nullable: true),
                    ShipmentId = table.Column<string>(nullable: true),
                    LineItemId = table.Column<string>(nullable: true),
                    PaymentInId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTaxDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderTaxDetail_OrderOperation_CustomerOrderId",
                        column: x => x.CustomerOrderId,
                        principalTable: "OrderOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderTaxDetail_OrderLineItem_LineItemId",
                        column: x => x.LineItemId,
                        principalTable: "OrderLineItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderTaxDetail_OrderOperation_PaymentInId",
                        column: x => x.PaymentInId,
                        principalTable: "OrderOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderTaxDetail_OrderOperation_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "OrderOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderShipmentItem",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    BarCode = table.Column<string>(maxLength: 128, nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    LineItemId = table.Column<string>(nullable: true),
                    ShipmentId = table.Column<string>(nullable: true),
                    ShipmentPackageId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderShipmentItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderShipmentItem_OrderLineItem_LineItemId",
                        column: x => x.LineItemId,
                        principalTable: "OrderLineItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderShipmentItem_OrderOperation_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "OrderOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderShipmentItem_OrderShipmentPackage_ShipmentPackageId",
                        column: x => x.ShipmentPackageId,
                        principalTable: "OrderShipmentPackage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderAddress_CustomerOrderId",
                table: "OrderAddress",
                column: "CustomerOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAddress_PaymentInId",
                table: "OrderAddress",
                column: "PaymentInId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAddress_ShipmentId",
                table: "OrderAddress",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDiscount_CustomerOrderId",
                table: "OrderDiscount",
                column: "CustomerOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDiscount_LineItemId",
                table: "OrderDiscount",
                column: "LineItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDiscount_PaymentInId",
                table: "OrderDiscount",
                column: "PaymentInId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDiscount_ShipmentId",
                table: "OrderDiscount",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLineItem_CustomerOrderId",
                table: "OrderLineItem",
                column: "CustomerOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOperation_CustomerOrderId",
                table: "OrderOperation",
                column: "CustomerOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOperation_ShipmentId",
                table: "OrderOperation",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOperation_ShipmentEntity_CustomerOrderId",
                table: "OrderOperation",
                column: "ShipmentEntity_CustomerOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPaymentGatewayTransaction_PaymentInId",
                table: "OrderPaymentGatewayTransaction",
                column: "PaymentInId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderShipmentItem_LineItemId",
                table: "OrderShipmentItem",
                column: "LineItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderShipmentItem_ShipmentId",
                table: "OrderShipmentItem",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderShipmentItem_ShipmentPackageId",
                table: "OrderShipmentItem",
                column: "ShipmentPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderShipmentPackage_ShipmentId",
                table: "OrderShipmentPackage",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTaxDetail_CustomerOrderId",
                table: "OrderTaxDetail",
                column: "CustomerOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTaxDetail_LineItemId",
                table: "OrderTaxDetail",
                column: "LineItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTaxDetail_PaymentInId",
                table: "OrderTaxDetail",
                column: "PaymentInId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTaxDetail_ShipmentId",
                table: "OrderTaxDetail",
                column: "ShipmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderAddress");

            migrationBuilder.DropTable(
                name: "OrderDiscount");

            migrationBuilder.DropTable(
                name: "OrderPaymentGatewayTransaction");

            migrationBuilder.DropTable(
                name: "OrderShipmentItem");

            migrationBuilder.DropTable(
                name: "OrderTaxDetail");

            migrationBuilder.DropTable(
                name: "OrderShipmentPackage");

            migrationBuilder.DropTable(
                name: "OrderLineItem");

            migrationBuilder.DropTable(
                name: "OrderOperation");
        }
    }
}
