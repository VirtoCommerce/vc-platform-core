using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.MarketingModule.Data.Migrations
{
    public partial class AddMarketingDynamicPropertyObjectValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DynamicContentItemDynamicPropertyObjectValue",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ObjectType = table.Column<string>(maxLength: 256, nullable: true),
                    ObjectId = table.Column<string>(maxLength: 128, nullable: true),
                    Locale = table.Column<string>(maxLength: 64, nullable: true),
                    ValueType = table.Column<string>(maxLength: 64, nullable: false),
                    ShortTextValue = table.Column<string>(maxLength: 512, nullable: true),
                    LongTextValue = table.Column<string>(nullable: true),
                    DecimalValue = table.Column<decimal>(type: "decimal(18,5)", nullable: true),
                    IntegerValue = table.Column<int>(nullable: true),
                    BooleanValue = table.Column<bool>(nullable: true),
                    DateTimeValue = table.Column<DateTime>(nullable: true),
                    PropertyId = table.Column<string>(maxLength: 128, nullable: true),
                    DictionaryItemId = table.Column<string>(maxLength: 128, nullable: true),
                    PropertyName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicContentItemDynamicPropertyObjectValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DynamicContentItemDynamicPropertyObjectValue_DynamicContentItem_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "DynamicContentItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DynamicContentItemDynamicPropertyObjectValue_ObjectId",
                table: "DynamicContentItemDynamicPropertyObjectValue",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectType_ObjectId",
                table: "DynamicContentItemDynamicPropertyObjectValue",
                columns: new[] { "ObjectType", "ObjectId" });

            migrationBuilder.Sql(@"IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '__MigrationHistory'))
                    BEGIN
                        INSERT INTO [dbo].[DynamicContentItemDynamicPropertyObjectValue] ([Id],[CreatedDate],[ModifiedDate],[CreatedBy],[ModifiedBy],[ObjectType],[ObjectId],[Locale],[ValueType],[ShortTextValue],[LongTextValue],[DecimalValue],[IntegerValue],[BooleanValue],[DateTimeValue],[PropertyId],[DictionaryItemId],[PropertyName])
                        SELECT OV.[Id],OV.[CreatedDate],OV.[ModifiedDate],OV.[CreatedBy],OV.[ModifiedBy],OV.[ObjectType],OV.[ObjectId],[Locale],OV.[ValueType],[ShortTextValue],[LongTextValue],[DecimalValue],[IntegerValue],[BooleanValue],[DateTimeValue],[PropertyId],[DictionaryItemId], PDP.[Name]
                        FROM [PlatformDynamicPropertyObjectValue] OV
						INNER JOIN [PlatformDynamicProperty] PDP ON PDP.Id = OV.PropertyId
                        WHERE OV.ObjectType = 'VirtoCommerce.MarketingModule.Core.Model.DynamicContentItem'
                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DynamicContentItemDynamicPropertyObjectValue");
        }
    }
}
