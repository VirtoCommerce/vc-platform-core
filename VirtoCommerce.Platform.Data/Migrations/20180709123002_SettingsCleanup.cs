using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.Platform.Data.Migrations
{
    public partial class SettingsCleanup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "PlatformSetting");

            migrationBuilder.DropColumn(
                name: "IsEnum",
                table: "PlatformSetting");

            migrationBuilder.DropColumn(
                name: "IsLocaleDependant",
                table: "PlatformSetting");

            migrationBuilder.DropColumn(
                name: "IsMultiValue",
                table: "PlatformSetting");

            migrationBuilder.DropColumn(
                name: "IsSystem",
                table: "PlatformSetting");

            migrationBuilder.DropColumn(
                name: "SettingValueType",
                table: "PlatformSetting");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PlatformSetting",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnum",
                table: "PlatformSetting",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocaleDependant",
                table: "PlatformSetting",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMultiValue",
                table: "PlatformSetting",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                table: "PlatformSetting",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SettingValueType",
                table: "PlatformSetting",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }
    }
}
