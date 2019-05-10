using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.CatalogModule.Data.Migrations
{
    public partial class UpdateCatalogV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '__MigrationHistory'))
                    BEGIN
                        BEGIN
	                        INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20190510073242_InitialCatalog', '2.2.3-servicing-35854')
                        END
                    END");

            migrationBuilder.CreateTable(
              name: "CatalogSeoInfo",
              columns: table => new
              {
                  Id = table.Column<string>(maxLength: 128, nullable: false),
                  CreatedDate = table.Column<DateTime>(nullable: false),
                  ModifiedDate = table.Column<DateTime>(nullable: true),
                  CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                  ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                  Keyword = table.Column<string>(maxLength: 255, nullable: false),
                  StoreId = table.Column<string>(maxLength: 128, nullable: true),
                  IsActive = table.Column<bool>(nullable: false),
                  Language = table.Column<string>(maxLength: 5, nullable: true),
                  Title = table.Column<string>(maxLength: 255, nullable: true),
                  MetaDescription = table.Column<string>(maxLength: 1024, nullable: true),
                  MetaKeywords = table.Column<string>(maxLength: 255, nullable: true),
                  ImageAltDescription = table.Column<string>(maxLength: 255, nullable: true),
                  ItemId = table.Column<string>(nullable: true),
                  CategoryId = table.Column<string>(nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_CatalogSeoInfo", x => x.Id);
                  table.ForeignKey(
                      name: "FK_CatalogSeoInfo_Category_CategoryId",
                      column: x => x.CategoryId,
                      principalTable: "Category",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
                  table.ForeignKey(
                      name: "FK_CatalogSeoInfo_Item_ItemId",
                      column: x => x.ItemId,
                      principalTable: "Item",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
              });

            migrationBuilder.CreateIndex(
              name: "IX_CatalogSeoInfo_CategoryId",
              table: "CatalogSeoInfo",
              column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogSeoInfo_ItemId",
                table: "CatalogSeoInfo",
                column: "ItemId");

            migrationBuilder.Sql(@"INSERT INTO [CatalogSeoInfo] ([Id], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy], [Keyword], [StoreId], [IsActive], [Language], [Title], [MetaDescription], [MetaKeywords], [ImageAltDescription], [CategoryId])
                              SELECT[Id], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy], [Keyword], [StoreId], [IsActive], [Language], [Title], [MetaDescription], [MetaKeywords], [ImageAltDescription], [ObjectId] as CategoryId FROM[SeoUrlKeyword] WHERE ObjectType = 'Category'");
            migrationBuilder.Sql(@" INSERT INTO [CatalogSeoInfo] ([Id], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy], [Keyword], [StoreId], [IsActive], [Language], [Title], [MetaDescription], [MetaKeywords], [ImageAltDescription], [ItemId])
                              SELECT [Id], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy], [Keyword], [StoreId], [IsActive], [Language], [Title], [MetaDescription], [MetaKeywords], [ImageAltDescription], [ObjectId] as ItemId FROM [SeoUrlKeyword] WHERE ObjectType = 'CatalogProduct'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
