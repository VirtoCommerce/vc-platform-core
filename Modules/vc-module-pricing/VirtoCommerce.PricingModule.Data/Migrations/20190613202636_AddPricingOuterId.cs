using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.PricingModule.Data.Migrations
{
    public partial class AddPricingOuterId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "PricelistAssignment",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "Pricelist",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "Price",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pricelist_OuterId",
                table: "Pricelist",
                column: "OuterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "PricelistAssignment");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "Pricelist");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "Price");
        }
    }
}
