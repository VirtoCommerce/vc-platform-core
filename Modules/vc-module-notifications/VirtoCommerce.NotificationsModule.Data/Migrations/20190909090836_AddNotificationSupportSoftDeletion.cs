using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.NotificationsModule.Data.Migrations
{
    public partial class AddNotificationSupportSoftDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Notification",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Notification");
        }
    }
}
