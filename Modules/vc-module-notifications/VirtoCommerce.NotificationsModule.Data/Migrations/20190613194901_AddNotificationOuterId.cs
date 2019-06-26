using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.NotificationsModule.Data.Migrations
{
    public partial class AddNotificationOuterId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "NotificationTemplate",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "NotificationMessage",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "NotificationEmailAttachment",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "Notification",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "NotificationTemplate");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "NotificationMessage");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "NotificationEmailAttachment");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "Notification");
        }
    }
}
