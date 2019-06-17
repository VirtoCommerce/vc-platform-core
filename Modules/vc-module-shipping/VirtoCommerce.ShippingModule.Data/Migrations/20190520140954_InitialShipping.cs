using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.ShippingModule.Data.Migrations
{
    public partial class InitialShipping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoreShippingMethod",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Code = table.Column<string>(maxLength: 128, nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    LogoUrl = table.Column<string>(maxLength: 2048, nullable: true),
                    TaxType = table.Column<string>(maxLength: 64, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    TypeName = table.Column<string>(maxLength: 128, nullable: true),
                    StoreId = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreShippingMethod", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoreShippingMethodEntity_TypeName_StoreId",
                table: "StoreShippingMethod",
                columns: new[] { "TypeName", "StoreId" },
                unique: true,
                filter: "[TypeName] IS NOT NULL AND [StoreId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreShippingMethod");
        }
    }
}
