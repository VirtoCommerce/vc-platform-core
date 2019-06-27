using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.CatalogModule.Data.Migrations
{
    public partial class AddCatalogOuterId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "PropertyValue",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "Property",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "Item",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "Category",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "CatalogImage",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "CatalogAsset",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "Catalog",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "Association",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "PropertyValue");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "Property");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "CatalogImage");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "CatalogAsset");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "Association");
        }
    }
}
