using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.CatalogModule.Data.Migrations
{
    public partial class CascadeDeleteCategoryRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryItemRelation_Item_ItemId",
                table: "CategoryItemRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryRelation_Category_SourceCategoryId",
                table: "CategoryRelation");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryItemRelation_Item_ItemId",
                table: "CategoryItemRelation",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryRelation_Category_SourceCategoryId",
                table: "CategoryRelation",
                column: "SourceCategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryItemRelation_Item_ItemId",
                table: "CategoryItemRelation");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryRelation_Category_SourceCategoryId",
                table: "CategoryRelation");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryItemRelation_Item_ItemId",
                table: "CategoryItemRelation",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryRelation_Category_SourceCategoryId",
                table: "CategoryRelation",
                column: "SourceCategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
