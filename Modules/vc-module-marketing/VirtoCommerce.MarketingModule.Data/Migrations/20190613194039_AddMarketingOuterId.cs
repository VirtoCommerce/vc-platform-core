using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.MarketingModule.Data.Migrations
{
    public partial class AddMarketingOuterId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "PublishingGroupContentPlace",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "PublishingGroupContentItem",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "PromotionUsage",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "Promotion",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "DynamicContentPublishingGroup",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "DynamicContentPlace",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "DynamicContentItem",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "DynamicContentFolder",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "Coupon",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "PublishingGroupContentPlace");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "PublishingGroupContentItem");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "PromotionUsage");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "DynamicContentPublishingGroup");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "DynamicContentPlace");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "DynamicContentItem");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "DynamicContentFolder");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "Coupon");
        }
    }
}
