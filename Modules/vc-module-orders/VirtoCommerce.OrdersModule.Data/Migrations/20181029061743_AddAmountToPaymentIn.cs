using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.OrdersModule.Data.Migrations
{
    public partial class AddAmountToPaymentIn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "OrderPaymentIn",
                type: "Money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql(@"UPDATE dbo.OrderPaymentIn SET Amount = Sum");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "OrderPaymentIn");
        }
    }
}
