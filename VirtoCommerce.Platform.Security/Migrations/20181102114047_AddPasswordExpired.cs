using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.Platform.Security.Migrations
{
    public partial class AddPasswordExpired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PasswordExpired",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordExpired",
                table: "AspNetUsers");
        }
    }
}
