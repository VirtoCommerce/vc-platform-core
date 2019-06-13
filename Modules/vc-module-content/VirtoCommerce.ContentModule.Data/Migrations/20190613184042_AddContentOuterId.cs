using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.ContentModule.Data.Migrations
{
    public partial class AddContentOuterId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "ContentMenuLinkList",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "ContentMenuLink",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "ContentMenuLinkList");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "ContentMenuLink");
        }
    }
}
