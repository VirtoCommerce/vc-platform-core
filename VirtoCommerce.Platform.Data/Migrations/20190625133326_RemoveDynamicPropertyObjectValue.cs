using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.Platform.Data.Migrations
{
    public partial class RemoveDynamicPropertyObjectValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlatformDynamicPropertyObjectValue");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlatformDynamicPropertyObjectValue",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    BooleanValue = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DateTimeValue = table.Column<DateTime>(nullable: true),
                    DecimalValue = table.Column<decimal>(type: "decimal(18,5)", nullable: true),
                    DictionaryItemId = table.Column<string>(nullable: true),
                    IntegerValue = table.Column<int>(nullable: true),
                    Locale = table.Column<string>(maxLength: 64, nullable: true),
                    LongTextValue = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ObjectId = table.Column<string>(maxLength: 128, nullable: true),
                    ObjectType = table.Column<string>(maxLength: 256, nullable: true),
                    PropertyId = table.Column<string>(nullable: true),
                    ShortTextValue = table.Column<string>(maxLength: 512, nullable: true),
                    ValueType = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformDynamicPropertyObjectValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlatformDynamicPropertyObjectValue_PlatformDynamicPropertyDictionaryItem_DictionaryItemId",
                        column: x => x.DictionaryItemId,
                        principalTable: "PlatformDynamicPropertyDictionaryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlatformDynamicPropertyObjectValue_PlatformDynamicProperty_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "PlatformDynamicProperty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlatformDynamicPropertyObjectValue_DictionaryItemId",
                table: "PlatformDynamicPropertyObjectValue",
                column: "DictionaryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformDynamicPropertyObjectValue_PropertyId",
                table: "PlatformDynamicPropertyObjectValue",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectType_ObjectId",
                table: "PlatformDynamicPropertyObjectValue",
                columns: new[] { "ObjectType", "ObjectId" });
        }
    }
}
