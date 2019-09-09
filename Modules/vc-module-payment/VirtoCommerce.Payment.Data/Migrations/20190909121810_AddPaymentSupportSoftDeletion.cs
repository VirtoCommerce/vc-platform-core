using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.PaymentModule.Data.Migrations
{
    public partial class AddPaymentSupportSoftDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StorePaymentMethodEntity_TypeName_StoreId",
                table: "StorePaymentMethod");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "StorePaymentMethod");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "StorePaymentMethod");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StorePaymentMethod",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_StorePaymentMethodEntity_TypeName_StoreId",
                table: "StorePaymentMethod",
                columns: new[] { "TypeName", "StoreId" },
                unique: true,
                filter: "[TypeName] IS NOT NULL AND [StoreId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StorePaymentMethodEntity_TypeName_StoreId",
                table: "StorePaymentMethod");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StorePaymentMethod");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "StorePaymentMethod",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "StorePaymentMethod",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StorePaymentMethodEntity_TypeName_StoreId",
                table: "StorePaymentMethod",
                columns: new[] { "TypeName", "StoreId" },
                unique: true,
                filter: "[StoreId] IS NOT NULL");
        }
    }
}
