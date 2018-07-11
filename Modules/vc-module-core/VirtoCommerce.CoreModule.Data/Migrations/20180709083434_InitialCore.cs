using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.CoreModule.Data.Migrations
{
    public partial class InitialCore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Code = table.Column<string>(maxLength: 16, nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    IsPrimary = table.Column<bool>(nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "Money", nullable: false),
                    Symbol = table.Column<string>(maxLength: 16, nullable: true),
                    CustomFormatting = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PackageType",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 254, nullable: false),
                    Length = table.Column<decimal>(nullable: false),
                    Width = table.Column<decimal>(nullable: false),
                    Height = table.Column<decimal>(nullable: false),
                    MeasureUnit = table.Column<string>(maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeoUrlKeyword",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Keyword = table.Column<string>(maxLength: 255, nullable: false),
                    StoreId = table.Column<string>(maxLength: 128, nullable: true),
                    ObjectId = table.Column<string>(maxLength: 255, nullable: false),
                    ObjectType = table.Column<string>(maxLength: 64, nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Language = table.Column<string>(maxLength: 5, nullable: true),
                    Title = table.Column<string>(maxLength: 255, nullable: true),
                    MetaDescription = table.Column<string>(maxLength: 1024, nullable: true),
                    MetaKeywords = table.Column<string>(maxLength: 255, nullable: true),
                    ImageAltDescription = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeoUrlKeyword", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sequence",
                columns: table => new
                {
                    ObjectType = table.Column<string>(maxLength: 256, nullable: false),
                    Value = table.Column<int>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sequence", x => x.ObjectType);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Code",
                table: "Currency",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_KeywordStoreId",
                table: "SeoUrlKeyword",
                columns: new[] { "Keyword", "StoreId" });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectIdAndObjectType",
                table: "SeoUrlKeyword",
                columns: new[] { "ObjectId", "ObjectType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "PackageType");

            migrationBuilder.DropTable(
                name: "SeoUrlKeyword");

            migrationBuilder.DropTable(
                name: "Sequence");
        }
    }
}
