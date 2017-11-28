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
                name: "PlatformAccount",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    AccountState = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsAdministrator = table.Column<bool>(nullable: false),
                    MemberId = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    StoreId = table.Column<string>(maxLength: 128, nullable: true),
                    UserName = table.Column<string>(maxLength: 128, nullable: false),
                    UserType = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformAccount", x => x.Id);
                });

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
                name: "PlatformPermission",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformPermission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlatformRole",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformRole", x => x.Id);
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
                name: "PlatformApiAccount",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    AccountId = table.Column<string>(nullable: true),
                    ApiAccountType = table.Column<int>(nullable: false),
                    AppId = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    SecretKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformApiAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlatformApiAccount_PlatformAccount_AccountId",
                        column: x => x.AccountId,
                        principalTable: "PlatformAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "PlatformRoleAssignment",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    AccountId = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    RoleId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformRoleAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlatformRoleAssignment_PlatformAccount_AccountId",
                        column: x => x.AccountId,
                        principalTable: "PlatformAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlatformRoleAssignment_PlatformRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "PlatformRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlatformRolePermission",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    PermissionId = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformRolePermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlatformRolePermission_PlatformPermission_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "PlatformPermission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlatformRolePermission_PlatformRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "PlatformRole",
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

            migrationBuilder.CreateTable(
                name: "PlatformPermissionScope",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Label = table.Column<string>(maxLength: 1024, nullable: true),
                    RolePermissionId = table.Column<string>(nullable: true),
                    Scope = table.Column<string>(maxLength: 1024, nullable: false),
                    Type = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformPermissionScope", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlatformPermissionScope_PlatformRolePermission_RolePermissionId",
                        column: x => x.RolePermissionId,
                        principalTable: "PlatformRolePermission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserName",
                table: "PlatformAccount",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlatformApiAccount_AccountId",
                table: "PlatformApiAccount",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AppId",
                table: "PlatformApiAccount",
                column: "AppId",
                unique: true);

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
                name: "IX_PlatformPermissionScope_RolePermissionId",
                table: "PlatformPermissionScope",
                column: "RolePermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformRoleAssignment_AccountId",
                table: "PlatformRoleAssignment",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformRoleAssignment_RoleId",
                table: "PlatformRoleAssignment",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformRolePermission_PermissionId",
                table: "PlatformRolePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformRolePermission_RoleId",
                table: "PlatformRolePermission",
                column: "RoleId");

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
                name: "PlatformApiAccount");

            migrationBuilder.DropTable(
                name: "PlatformDynamicPropertyDictionaryItemName");

            migrationBuilder.DropTable(
                name: "PlatformDynamicPropertyName");

            migrationBuilder.DropTable(
                name: "PlatformDynamicPropertyObjectValue");

            migrationBuilder.DropTable(
                name: "PlatformOperationLog");

            migrationBuilder.DropTable(
                name: "PlatformPermissionScope");

            migrationBuilder.DropTable(
                name: "PlatformRoleAssignment");

            migrationBuilder.DropTable(
                name: "PlatformSettingValue");

            migrationBuilder.DropTable(
                name: "PlatformDynamicPropertyDictionaryItem");

            migrationBuilder.DropTable(
                name: "PlatformRolePermission");

            migrationBuilder.DropTable(
                name: "PlatformAccount");

            migrationBuilder.DropTable(
                name: "PlatformSetting");

            migrationBuilder.DropTable(
                name: "PlatformDynamicProperty");

            migrationBuilder.DropTable(
                name: "PlatformPermission");

            migrationBuilder.DropTable(
                name: "PlatformRole");
        }
    }
}
