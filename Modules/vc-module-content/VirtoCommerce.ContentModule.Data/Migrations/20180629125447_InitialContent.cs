using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.ContentModule.Data.Migrations
{
    public partial class InitialContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentMenuLinkList",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(nullable: false),
                    StoreId = table.Column<string>(nullable: false),
                    Language = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentMenuLinkList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentMenuLink",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Title = table.Column<string>(maxLength: 1024, nullable: false),
                    Url = table.Column<string>(maxLength: 2048, nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    AssociatedObjectType = table.Column<string>(maxLength: 254, nullable: true),
                    AssociatedObjectName = table.Column<string>(maxLength: 254, nullable: true),
                    AssociatedObjectId = table.Column<string>(maxLength: 128, nullable: true),
                    MenuLinkListId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentMenuLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentMenuLink_ContentMenuLinkList_MenuLinkListId",
                        column: x => x.MenuLinkListId,
                        principalTable: "ContentMenuLinkList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentMenuLink_MenuLinkListId",
                table: "ContentMenuLink",
                column: "MenuLinkListId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentMenuLink");

            migrationBuilder.DropTable(
                name: "ContentMenuLinkList");
        }
    }
}
