using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.SubscriptionModule.Data.Migrations
{
    public partial class AddSubscriptionSupportSoftDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Subscription",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Subscription");
        }
    }
}
