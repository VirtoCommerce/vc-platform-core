using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.PaymentModule.Data.Migrations
{
    public partial class InitialPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StorePaymentMethod",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Code = table.Column<string>(maxLength: 128, nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LogoUrl = table.Column<string>(maxLength: 2048, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsAvailableForPartial = table.Column<bool>(nullable: false),
                    TypeName = table.Column<string>(maxLength: 128, nullable: true),
                    StoreId = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorePaymentMethod", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StorePaymentMethodEntity_TypeName_StoreId",
                table: "StorePaymentMethod",
                columns: new[] { "TypeName", "StoreId" },
                unique: true,
                filter: "[StoreId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StorePaymentMethod");
        }
    }
}
