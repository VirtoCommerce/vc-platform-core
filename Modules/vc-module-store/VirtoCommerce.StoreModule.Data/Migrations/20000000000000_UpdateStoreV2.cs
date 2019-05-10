using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.StoreModule.Data.Migrations
{
    public partial class UpdateStoreV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '__MigrationHistory'))
                 BEGIN
                    BEGIN
	                    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20190510074155_InitialStore', '2.2.3-servicing-35854')
                    END               
                END");

            migrationBuilder.Sql(@"UPDATE [PlatformDynamicProperty] SET ObjectType = 'VirtoCommerce.StoreModule.Core.Model.Store' WHERE [Name] = 'FilteredBrowsing' AND ObjectType = 'VirtoCommerce.Domain.Store.Model.Store'");

            migrationBuilder.CreateTable(
              name: "StoreSeoInfo",
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
                  ImageAltDescription = table.Column<string>(maxLength: 255, nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_StoreSeoInfo", x => x.Id);
                  table.ForeignKey(
                      name: "FK_StoreSeoInfo_Store_StoreId",
                      column: x => x.StoreId,
                      principalTable: "Store",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
              });

            migrationBuilder.CreateIndex(
              name: "IX_StoreSeoInfo_StoreId",
              table: "StoreSeoInfo",
              column: "StoreId");


            migrationBuilder.Sql(@" INSERT INTO [StoreSeoInfo] ([Id], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy], [Keyword], [StoreId], [IsActive], [Language], [Title], [MetaDescription], [MetaKeywords], [ImageAltDescription])
                          SELECT [Id], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy], [Keyword], [ObjectId] as [StoreId], [IsActive], [Language], [Title], [MetaDescription], [MetaKeywords], [ImageAltDescription]  FROM [SeoUrlKeyword] WHERE ObjectType = 'Store'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
