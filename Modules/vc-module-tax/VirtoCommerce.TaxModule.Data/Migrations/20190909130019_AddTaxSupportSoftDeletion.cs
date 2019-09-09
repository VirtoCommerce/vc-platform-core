using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.TaxModule.Data.Migrations
{
    public partial class AddTaxSupportSoftDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StoreTaxProvider",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StoreTaxProvider");
        }
    }
}
