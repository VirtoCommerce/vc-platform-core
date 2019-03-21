using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.LicensingModule.Data.Migrations
{
    public partial class InitialLicensing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "License",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Type = table.Column<string>(maxLength: 64, nullable: false),
                    CustomerName = table.Column<string>(maxLength: 256, nullable: false),
                    CustomerEmail = table.Column<string>(maxLength: 256, nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: false),
                    ActivationCode = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_License", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivationCode",
                table: "License",
                column: "ActivationCode",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "License");
        }
    }
}
