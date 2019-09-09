using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.ShippingModule.Data.Migrations
{
    public partial class AddShippingSupportSoftDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StoreShippingMethod",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StoreShippingMethod");
        }
    }
}
