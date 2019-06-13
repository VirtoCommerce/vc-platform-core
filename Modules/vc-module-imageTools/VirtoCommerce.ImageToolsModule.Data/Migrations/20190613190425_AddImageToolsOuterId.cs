using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.ImageToolsModule.Data.Migrations
{
    public partial class AddImageToolsOuterId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ThumbnailTaskId",
                table: "ThumbnailTaskOption",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ThumbnailOptionId",
                table: "ThumbnailTaskOption",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "ThumbnailTask",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "ThumbnailOption",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "ThumbnailTask");

            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "ThumbnailOption");

            migrationBuilder.AlterColumn<string>(
                name: "ThumbnailTaskId",
                table: "ThumbnailTaskOption",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ThumbnailOptionId",
                table: "ThumbnailTaskOption",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
