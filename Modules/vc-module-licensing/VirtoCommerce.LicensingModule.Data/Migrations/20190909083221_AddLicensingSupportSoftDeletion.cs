using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.LicensingModule.Data.Migrations
{
    public partial class AddLicensingSupportSoftDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "License",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "License");
        }
    }
}
