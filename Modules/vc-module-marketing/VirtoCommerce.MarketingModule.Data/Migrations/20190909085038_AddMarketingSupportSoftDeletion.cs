using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.MarketingModule.Data.Migrations
{
    public partial class AddMarketingSupportSoftDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Promotion",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DynamicContentPublishingGroup",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DynamicContentPlace",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DynamicContentItem",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DynamicContentPublishingGroup");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DynamicContentPlace");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DynamicContentItem");
        }
    }
}
