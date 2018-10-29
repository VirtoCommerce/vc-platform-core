using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.CartModule.Data.Migrations
{
    public partial class MultipleCouponsPerCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartCoupon",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Code = table.Column<string>(maxLength: 64, nullable: true),
                    ShoppingCartId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartCoupon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartCoupon_Cart_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "Cart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartCoupon_ShoppingCartId",
                table: "CartCoupon",
                column: "ShoppingCartId");

            migrationBuilder.Sql("INSERT INTO dbo.CartCoupon (Id, Code, ShoppingCartId) SELECT cast(LOWER(REPLACE( NEWID(), '-', '')) as nvarchar(128)), Coupon, Id FROM dbo.Cart WHERE Coupon IS NOT NULL");

            migrationBuilder.DropColumn(
                name: "Coupon",
                table: "Cart");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartCoupon");

            migrationBuilder.AddColumn<string>(
                name: "Coupon",
                table: "Cart",
                maxLength: 64,
                nullable: true);
        }
    }
}
