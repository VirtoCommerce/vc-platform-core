using Microsoft.EntityFrameworkCore.Migrations;

namespace Module1.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(name: "Discriminator", table: "PlatformSetting", nullable: false, maxLength: 128, defaultValue: "SettingEntity2");
            migrationBuilder.AddColumn<string>(name: "NewField", table: "PlatformSetting", maxLength: 1024);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
