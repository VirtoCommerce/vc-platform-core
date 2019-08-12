using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.NotificationsSampleModule.Web.Migrations
{
    public partial class InitialTwitterNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(name: "Post", table: "Notification", maxLength: 1024, nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Post", table: "Notification");
        }
    }
}
