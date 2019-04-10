using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.SubscriptionModule.Data.Migrations
{
    public partial class InitialSubscription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentPlan",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Interval = table.Column<string>(maxLength: 64, nullable: true),
                    IntervalCount = table.Column<int>(nullable: false),
                    TrialPeriodDays = table.Column<int>(nullable: false),
                    ProductId = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentPlan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscription",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    StoreId = table.Column<string>(maxLength: 64, nullable: false),
                    CustomerId = table.Column<string>(maxLength: 64, nullable: false),
                    CustomerName = table.Column<string>(maxLength: 255, nullable: true),
                    Number = table.Column<string>(maxLength: 64, nullable: false),
                    Balance = table.Column<decimal>(type: "Money", nullable: false),
                    Interval = table.Column<string>(maxLength: 64, nullable: true),
                    IntervalCount = table.Column<int>(nullable: false),
                    TrialPeriodDays = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    TrialSart = table.Column<DateTime>(nullable: true),
                    TrialEnd = table.Column<DateTime>(nullable: true),
                    CurrentPeriodStart = table.Column<DateTime>(nullable: true),
                    CurrentPeriodEnd = table.Column<DateTime>(nullable: true),
                    Status = table.Column<string>(maxLength: 64, nullable: true),
                    IsCancelled = table.Column<bool>(nullable: false),
                    CancelledDate = table.Column<DateTime>(nullable: true),
                    CancelReason = table.Column<string>(maxLength: 2048, nullable: true),
                    CustomerOrderPrototypeId = table.Column<string>(nullable: true),
                    OuterId = table.Column<string>(maxLength: 256, nullable: true),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentPlan");

            migrationBuilder.DropTable(
                name: "Subscription");
        }
    }
}
