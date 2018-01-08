using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace VirtoCommerce.Platform.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlatformDynamicProperty",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    DisplayOrder = table.Column<int>(nullable: true),
                    IsArray = table.Column<bool>(nullable: false),
                    IsDictionary = table.Column<bool>(nullable: false),
                    IsMultilingual = table.Column<bool>(nullable: false),
                    IsRequired = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    ObjectType = table.Column<string>(maxLength: 256, nullable: true),
                    ValueType = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformDynamicProperty", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlatformOperationLog",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Detail = table.Column<string>(maxLength: 1024, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ObjectId = table.Column<string>(maxLength: 200, nullable: true),
                    ObjectType = table.Column<string>(maxLength: 50, nullable: true),
                    OperationType = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformOperationLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlatformSetting",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 1024, nullable: true),
                    IsEnum = table.Column<bool>(nullable: false),
                    IsLocaleDependant = table.Column<bool>(nullable: false),
                    IsMultiValue = table.Column<bool>(nullable: false),
                    IsSystem = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    ObjectId = table.Column<string>(maxLength: 128, nullable: true),
                    ObjectType = table.Column<string>(maxLength: 128, nullable: true),
                    SettingValueType = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformSetting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlatformDynamicPropertyDictionaryItem",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: true),
                    PropertyId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformDynamicPropertyDictionaryItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlatformDynamicPropertyDictionaryItem_PlatformDynamicProperty_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "PlatformDynamicProperty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlatformDynamicPropertyName",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Locale = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    PropertyId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformDynamicPropertyName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlatformDynamicPropertyName_PlatformDynamicProperty_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "PlatformDynamicProperty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlatformSettingValue",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    BooleanValue = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DateTimeValue = table.Column<DateTime>(nullable: true),
                    DecimalValue = table.Column<decimal>(nullable: false),
                    IntegerValue = table.Column<int>(nullable: false),
                    Locale = table.Column<string>(maxLength: 64, nullable: true),
                    LongTextValue = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    SettingId = table.Column<string>(nullable: true),
                    ShortTextValue = table.Column<string>(maxLength: 512, nullable: true),
                    ValueType = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformSettingValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlatformSettingValue_PlatformSetting_SettingId",
                        column: x => x.SettingId,
                        principalTable: "PlatformSetting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlatformDynamicPropertyDictionaryItemName",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DictionaryItemId = table.Column<string>(nullable: true),
                    Locale = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformDynamicPropertyDictionaryItemName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlatformDynamicPropertyDictionaryItemName_PlatformDynamicPropertyDictionaryItem_DictionaryItemId",
                        column: x => x.DictionaryItemId,
                        principalTable: "PlatformDynamicPropertyDictionaryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlatformDynamicPropertyObjectValue",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    BooleanValue = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DateTimeValue = table.Column<DateTime>(nullable: true),
                    DecimalValue = table.Column<decimal>(nullable: true),
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
                name: "IX_PlatformDynamicProperty_ObjectType_Name",
                table: "PlatformDynamicProperty",
                columns: new[] { "ObjectType", "Name" },
                unique: true,
                filter: "[ObjectType] IS NOT NULL AND [Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformDynamicPropertyDictionaryItem_PropertyId_Name",
                table: "PlatformDynamicPropertyDictionaryItem",
                columns: new[] { "PropertyId", "Name" },
                unique: true,
                filter: "[PropertyId] IS NOT NULL AND [Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformDynamicPropertyDictionaryItemName_DictionaryItemId_Locale_Name",
                table: "PlatformDynamicPropertyDictionaryItemName",
                columns: new[] { "DictionaryItemId", "Locale", "Name" },
                unique: true,
                filter: "[DictionaryItemId] IS NOT NULL AND [Locale] IS NOT NULL AND [Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformDynamicPropertyName_PropertyId_Locale_Name",
                table: "PlatformDynamicPropertyName",
                columns: new[] { "PropertyId", "Locale", "Name" },
                unique: true,
                filter: "[PropertyId] IS NOT NULL AND [Locale] IS NOT NULL AND [Name] IS NOT NULL");

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

            migrationBuilder.CreateIndex(
                name: "IX_ObjectType_ObjectId",
                table: "PlatformOperationLog",
                columns: new[] { "ObjectType", "ObjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectType_ObjectId",
                table: "PlatformSetting",
                columns: new[] { "ObjectType", "ObjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_PlatformSettingValue_SettingId",
                table: "PlatformSettingValue",
                column: "SettingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlatformDynamicPropertyDictionaryItemName");

            migrationBuilder.DropTable(
                name: "PlatformDynamicPropertyName");

            migrationBuilder.DropTable(
                name: "PlatformDynamicPropertyObjectValue");

            migrationBuilder.DropTable(
                name: "PlatformOperationLog");

            migrationBuilder.DropTable(
                name: "PlatformSettingValue");

            migrationBuilder.DropTable(
                name: "PlatformDynamicPropertyDictionaryItem");

            migrationBuilder.DropTable(
                name: "PlatformSetting");

            migrationBuilder.DropTable(
                name: "PlatformDynamicProperty");
        }
    }
}
