﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.SubscriptionModule.Data.Migrations
{
    public partial class AddSubscriptionOuterId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OuterId",
                table: "Subscription",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OuterId",
                table: "PaymentPlan",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OuterId",
                table: "PaymentPlan");

            migrationBuilder.AlterColumn<string>(
                name: "OuterId",
                table: "Subscription",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128,
                oldNullable: true);
        }
    }
}