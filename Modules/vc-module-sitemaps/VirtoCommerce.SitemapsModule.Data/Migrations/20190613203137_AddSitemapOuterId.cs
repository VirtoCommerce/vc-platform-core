using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.SitemapsModule.Data.Migrations
{
    public partial class AddSitemapOuterId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "SitemapItem",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "Sitemap",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "SitemapItem");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "Sitemap");
        }
    }
}
