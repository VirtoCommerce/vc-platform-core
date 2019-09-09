using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.StoreModule.Data.Migrations
{
    public partial class AddStoreSupportSoftDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Store",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Store");
        }
    }
}
