using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.ImageToolsModule.Data.Migrations
{
    public partial class InitialImageTools : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThumbnailOption",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 1024, nullable: false),
                    FileSuffix = table.Column<string>(maxLength: 128, nullable: false),
                    ResizeMethod = table.Column<string>(maxLength: 64, nullable: false),
                    Width = table.Column<int>(nullable: true),
                    Height = table.Column<int>(nullable: true),
                    BackgroundColor = table.Column<string>(nullable: true),
                    AnchorPosition = table.Column<string>(maxLength: 64, nullable: true),
                    JpegQuality = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThumbnailOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThumbnailTask",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 1024, nullable: false),
                    WorkPath = table.Column<string>(maxLength: 2048, nullable: false),
                    LastRun = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThumbnailTask", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThumbnailTaskOption",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    ThumbnailTaskId = table.Column<string>(nullable: false),
                    ThumbnailOptionId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThumbnailTaskOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThumbnailTaskOption_ThumbnailOption_ThumbnailOptionId",
                        column: x => x.ThumbnailOptionId,
                        principalTable: "ThumbnailOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThumbnailTaskOption_ThumbnailTask_ThumbnailTaskId",
                        column: x => x.ThumbnailTaskId,
                        principalTable: "ThumbnailTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThumbnailTaskOption_ThumbnailOptionId",
                table: "ThumbnailTaskOption",
                column: "ThumbnailOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ThumbnailTaskOption_ThumbnailTaskId",
                table: "ThumbnailTaskOption",
                column: "ThumbnailTaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThumbnailTaskOption");

            migrationBuilder.DropTable(
                name: "ThumbnailOption");

            migrationBuilder.DropTable(
                name: "ThumbnailTask");
        }
    }
}
