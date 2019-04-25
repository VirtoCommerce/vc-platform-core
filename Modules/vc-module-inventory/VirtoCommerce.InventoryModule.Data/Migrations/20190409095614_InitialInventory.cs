using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.InventoryModule.Data.Migrations
{
    public partial class InitialInventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FulfillmentCenter",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Line1 = table.Column<string>(maxLength: 1024, nullable: true),
                    Line2 = table.Column<string>(maxLength: 1024, nullable: true),
                    City = table.Column<string>(maxLength: 128, nullable: true),
                    CountryCode = table.Column<string>(maxLength: 64, nullable: true),
                    StateProvince = table.Column<string>(maxLength: 128, nullable: true),
                    CountryName = table.Column<string>(maxLength: 128, nullable: true),
                    PostalCode = table.Column<string>(maxLength: 32, nullable: true),
                    RegionId = table.Column<string>(maxLength: 128, nullable: true),
                    RegionName = table.Column<string>(maxLength: 128, nullable: true),
                    DaytimePhoneNumber = table.Column<string>(maxLength: 64, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    Organization = table.Column<string>(maxLength: 128, nullable: true),
                    GeoLocation = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FulfillmentCenter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    InStockQuantity = table.Column<decimal>(nullable: false),
                    ReservedQuantity = table.Column<decimal>(nullable: false),
                    ReorderMinQuantity = table.Column<decimal>(nullable: false),
                    PreorderQuantity = table.Column<decimal>(nullable: false),
                    BackorderQuantity = table.Column<decimal>(nullable: false),
                    AllowBackorder = table.Column<bool>(nullable: false),
                    AllowPreorder = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    PreorderAvailabilityDate = table.Column<DateTime>(nullable: true),
                    BackorderAvailabilityDate = table.Column<DateTime>(nullable: true),
                    Sku = table.Column<string>(maxLength: 128, nullable: false),
                    FulfillmentCenterId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventory_FulfillmentCenter_FulfillmentCenterId",
                        column: x => x.FulfillmentCenterId,
                        principalTable: "FulfillmentCenter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_FulfillmentCenterId",
                table: "Inventory",
                column: "FulfillmentCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_Sku",
                table: "Inventory",
                column: "Sku");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "FulfillmentCenter");
        }
    }
}
