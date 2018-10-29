using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.OrdersModule.Data.Migrations
{
    public partial class AddAddressName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "OrderAddress",
                maxLength: 2048,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "OrderAddress");
        }
    }
}
