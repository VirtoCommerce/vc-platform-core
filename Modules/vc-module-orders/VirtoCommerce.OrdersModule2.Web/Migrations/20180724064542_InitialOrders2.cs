using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.OrdersModule2.Web.Migrations
{
    public partial class InitialOrders2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(name: "NewField", table: "CustomerOrder", maxLength: 128, nullable: true);
            migrationBuilder.AddColumn<string>(name: "Discriminator", table: "CustomerOrder", nullable: false, maxLength: 128, defaultValue: "CustomerOrderEntity");

            migrationBuilder.CreateTable(
                name: "OrderInvoice",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CancelReason = table.Column<string>(maxLength: 2048, nullable: true),
                    CancelledDate = table.Column<DateTime>(nullable: true),
                    Comment = table.Column<string>(maxLength: 2048, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Currency = table.Column<string>(maxLength: 3, nullable: false),
                    IsApproved = table.Column<bool>(nullable: false),
                    IsCancelled = table.Column<bool>(nullable: false),
                    Number = table.Column<string>(maxLength: 64, nullable: false),
                    Status = table.Column<string>(maxLength: 64, nullable: true),
                    Sum = table.Column<decimal>(type: "Money", nullable: false),
                    CustomerId = table.Column<string>(nullable: true),
                    CustomerName = table.Column<string>(nullable: true),
                    CustomerOrder2Id = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderInvoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderInvoice_CustomerOrder_CustomerOrder2Id",
                        column: x => x.CustomerOrder2Id,
                        principalTable: "CustomerOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("NewField", "CustomerOrder");
            migrationBuilder.DropColumn("Discriminator", "CustomerOrder");
            migrationBuilder.DropTable("dbo.OrderInvoice");
        }
    }
}
