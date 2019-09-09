using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.ContentModule.Data.Migrations
{
    public partial class AddContentSupportSoftDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ContentMenuLink",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ContentMenuLink");
        }
    }
}
