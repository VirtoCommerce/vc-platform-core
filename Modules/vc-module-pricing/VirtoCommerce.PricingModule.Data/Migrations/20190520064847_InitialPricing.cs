using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.PricingModule.Data.Migrations
{
    public partial class InitialPricing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pricelist",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    Currency = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pricelist", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Price",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Sale = table.Column<decimal>(type: "Money", nullable: true),
                    List = table.Column<decimal>(type: "Money", nullable: false),
                    ProductId = table.Column<string>(maxLength: 128, nullable: true),
                    ProductName = table.Column<string>(maxLength: 1024, nullable: true),
                    MinQuantity = table.Column<decimal>(nullable: false),
                    PricelistId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Price", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Price_Pricelist_PricelistId",
                        column: x => x.PricelistId,
                        principalTable: "Pricelist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PricelistAssignment",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    ConditionExpression = table.Column<string>(nullable: true),
                    PredicateVisualTreeSerialized = table.Column<string>(nullable: true),
                    CatalogId = table.Column<string>(maxLength: 128, nullable: false),
                    PricelistId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricelistAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PricelistAssignment_Pricelist_PricelistId",
                        column: x => x.PricelistId,
                        principalTable: "Pricelist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Price_PricelistId",
                table: "Price",
                column: "PricelistId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceId",
                table: "Price",
                columns: new[] { "ProductId", "PricelistId" });

            migrationBuilder.CreateIndex(
                name: "IX_PricelistAssignment_PricelistId",
                table: "PricelistAssignment",
                column: "PricelistId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Price");

            migrationBuilder.DropTable(
                name: "PricelistAssignment");

            migrationBuilder.DropTable(
                name: "Pricelist");
        }
    }
}
