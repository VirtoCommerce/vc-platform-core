using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.PricingModule.Data.Migrations
{
    public partial class UpdatePricingV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__MigrationHistory'))
                IF (EXISTS (SELECT * FROM __MigrationHistory WHERE ContextKey = 'VirtoCommerce.PricingModule.Data.Migrations.Configuration'))
                    BEGIN
	                    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20190520064847_InitialPricing', '2.2.3-servicing-35854')
                        UPDATE [PricelistAssignment] SET [PredicateVisualTreeSerialized] = REPLACE([PredicateVisualTreeSerialized], 'ConditionExpressionTree', 'PriceConditionTree')
                        UPDATE [PricelistAssignment] SET [ConditionExpression] = null

                        BEGIN
                            DECLARE @typeInd varchar(10) = '""$type"":""';
                            DECLARE @commaInd varchar(10) = '"",';
                            DECLARE @index INT = 1;

                            WHILE @index < 20
                            BEGIN
                                UPDATE [PricelistAssignment] SET[PredicateVisualTreeSerialized] = REPLACE([PredicateVisualTreeSerialized], SUBSTRING([PredicateVisualTreeSerialized], CHARINDEX(@typeInd, [PredicateVisualTreeSerialized]), CHARINDEX(@commaInd, [PredicateVisualTreeSerialized], CHARINDEX(@typeInd, [PredicateVisualTreeSerialized])) - CHARINDEX(@typeInd, [PredicateVisualTreeSerialized]) + 2), '')
                                WHERE CHARINDEX(@typeInd, [PredicateVisualTreeSerialized]) > 0
                                SET @index = @index + 1;
                            END
                        END
                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
