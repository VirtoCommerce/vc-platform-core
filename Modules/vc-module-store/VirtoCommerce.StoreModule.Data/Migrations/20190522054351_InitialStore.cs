using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.StoreModule.Data.Migrations
{
    public partial class InitialStore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Store",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    Url = table.Column<string>(maxLength: 256, nullable: true),
                    StoreState = table.Column<int>(nullable: false),
                    TimeZone = table.Column<string>(maxLength: 128, nullable: true),
                    Country = table.Column<string>(maxLength: 128, nullable: true),
                    Region = table.Column<string>(maxLength: 128, nullable: true),
                    DefaultLanguage = table.Column<string>(maxLength: 128, nullable: true),
                    DefaultCurrency = table.Column<string>(maxLength: 64, nullable: true),
                    Catalog = table.Column<string>(maxLength: 128, nullable: false),
                    CreditCardSavePolicy = table.Column<int>(nullable: false),
                    SecureUrl = table.Column<string>(maxLength: 128, nullable: true),
                    Email = table.Column<string>(maxLength: 128, nullable: true),
                    AdminEmail = table.Column<string>(maxLength: 128, nullable: true),
                    DisplayOutOfStock = table.Column<bool>(nullable: false),
                    FulfillmentCenterId = table.Column<string>(maxLength: 128, nullable: true),
                    ReturnsFulfillmentCenterId = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoreCurrency",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CurrencyCode = table.Column<string>(maxLength: 32, nullable: false),
                    StoreId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreCurrency", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreCurrency_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreFulfillmentCenter",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Type = table.Column<string>(maxLength: 32, nullable: false),
                    FulfillmentCenterId = table.Column<string>(maxLength: 128, nullable: false),
                    StoreId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreFulfillmentCenter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreFulfillmentCenter_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreLanguage",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    LanguageCode = table.Column<string>(maxLength: 32, nullable: false),
                    StoreId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreLanguage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreLanguage_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreSeoInfo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Keyword = table.Column<string>(maxLength: 255, nullable: false),
                    StoreId = table.Column<string>(maxLength: 128, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Language = table.Column<string>(maxLength: 5, nullable: true),
                    Title = table.Column<string>(maxLength: 255, nullable: true),
                    MetaDescription = table.Column<string>(maxLength: 1024, nullable: true),
                    MetaKeywords = table.Column<string>(maxLength: 255, nullable: true),
                    ImageAltDescription = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreSeoInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreSeoInfo_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreTrustedGroup",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    GroupName = table.Column<string>(maxLength: 128, nullable: false),
                    StoreId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreTrustedGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreTrustedGroup_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoreCurrency_StoreId",
                table: "StoreCurrency",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreFulfillmentCenter_StoreId",
                table: "StoreFulfillmentCenter",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreLanguage_StoreId",
                table: "StoreLanguage",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreSeoInfo_StoreId",
                table: "StoreSeoInfo",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTrustedGroup_StoreId",
                table: "StoreTrustedGroup",
                column: "StoreId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreCurrency");

            migrationBuilder.DropTable(
                name: "StoreFulfillmentCenter");

            migrationBuilder.DropTable(
                name: "StoreLanguage");

            migrationBuilder.DropTable(
                name: "StoreSeoInfo");

            migrationBuilder.DropTable(
                name: "StoreTrustedGroup");

            migrationBuilder.DropTable(
                name: "Store");
        }
    }
}
