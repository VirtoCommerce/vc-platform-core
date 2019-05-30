using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.PricingModule.Data.Migrations
{
    public partial class UpdatePricingV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '__MigrationHistory'))
                    BEGIN
	                    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20190520064847_InitialPricing', '2.2.3-servicing-35854')
                        UPDATE [PricelistAssignment] SET [PredicateVisualTreeSerialized] = REPLACE([PredicateVisualTreeSerialized], 'ConditionExpressionTree', 'PriceConditionTree')
                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
