using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.SitemapsModule.Data.Migrations
{
    public partial class AddSitemapsSupportSoftDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SitemapItem",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Sitemap",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SitemapItem");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Sitemap");
        }
    }
}
