using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.CustomerSampleModule.Web.Migrations
{
    public partial class InitialCustomerSample : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(name: "ContractNumber", table: "Member", maxLength: 128, nullable: true);
            migrationBuilder.AddColumn<string>(name: "JobTitle", table: "Member", maxLength: 128, nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ContractNumber", table: "Member");
            migrationBuilder.DropColumn(name: "JobTitle", table: "Member");
        }
    }
}
