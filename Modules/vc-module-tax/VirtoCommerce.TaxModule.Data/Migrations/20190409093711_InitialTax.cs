using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.TaxModule.Data.Migrations
{
    public partial class InitialTax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoreTaxProvider",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Code = table.Column<string>(maxLength: 128, nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    TypeName = table.Column<string>(maxLength: 128, nullable: false),
                    LogoUrl = table.Column<string>(maxLength: 2048, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    StoreId = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreTaxProvider", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoreTaxProviderEntity_TypeName_StoreId",
                table: "StoreTaxProvider",
                columns: new[] { "TypeName", "StoreId" },
                unique: true,
                filter: "[StoreId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreTaxProvider");
        }
    }
}
