using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.OrdersModule.Data.Migrations
{
    public partial class AddOuterId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "CustomerOrder",
                maxLength: 64,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "CustomerOrder");
        }
    }
}
