using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.StoreModule.Data.Migrations
{
    public partial class RemoveTaxProvider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreTaxProvider");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoreTaxProvider",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Code = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    LogoUrl = table.Column<string>(maxLength: 2048, nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    StoreId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreTaxProvider", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreTaxProvider_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoreTaxProvider_StoreId",
                table: "StoreTaxProvider",
                column: "StoreId");
        }
    }
}
