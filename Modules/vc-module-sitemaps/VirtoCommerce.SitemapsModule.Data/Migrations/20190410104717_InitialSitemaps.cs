using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.SitemapsModule.Data.Migrations
{
    public partial class InitialSitemaps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sitemap",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Filename = table.Column<string>(maxLength: 256, nullable: false),
                    StoreId = table.Column<string>(maxLength: 64, nullable: false),
                    UrlTemplate = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sitemap", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SitemapItem",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Title = table.Column<string>(maxLength: 256, nullable: false),
                    ImageUrl = table.Column<string>(maxLength: 512, nullable: true),
                    ObjectId = table.Column<string>(maxLength: 128, nullable: true),
                    ObjectType = table.Column<string>(maxLength: 128, nullable: false),
                    SitemapId = table.Column<string>(nullable: false),
                    UrlTemplate = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SitemapItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SitemapItem_Sitemap_SitemapId",
                        column: x => x.SitemapId,
                        principalTable: "Sitemap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sitemap_Filename",
                table: "Sitemap",
                column: "Filename");

            migrationBuilder.CreateIndex(
                name: "IX_SitemapItem_SitemapId",
                table: "SitemapItem",
                column: "SitemapId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SitemapItem");

            migrationBuilder.DropTable(
                name: "Sitemap");
        }
    }
}
