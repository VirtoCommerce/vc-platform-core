using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.MarketingModule.Data.Migrations
{
    public partial class UpdateMarketingV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__MigrationHistory'))
                IF (EXISTS (SELECT * FROM __MigrationHistory WHERE ContextKey = 'VirtoCommerce.MarketingModule.Data.Migrations.Configuration'))
                    BEGIN
	                    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20190409185706_InitialMarketing', '2.2.3-servicing-35854')

                        UPDATE [PlatformDynamicProperty] SET ObjectType = 'VirtoCommerce.MarketingModule.Core.Model.DynamicContentItem' WHERE ObjectType = 'VirtoCommerce.Domain.Marketing.Model.DynamicContentItem'
                        UPDATE [PlatformDynamicPropertyObjectValue] SET ObjectType = 'VirtoCommerce.MarketingModule.Core.Model.DynamicContentItem' WHERE ObjectType = 'VirtoCommerce.Domain.Marketing.Model.DynamicContentItem'

                        UPDATE [Promotion] SET [PredicateVisualTreeSerialized] = REPLACE([PredicateVisualTreeSerialized], 'PromoDynamicExpressionTree', 'PromotionConditionAndRewardTree')
                        UPDATE [Promotion] SET [PredicateVisualTreeSerialized] = REPLACE([PredicateVisualTreeSerialized], 'RewardBlock', 'BlockReward')
                        UPDATE [Promotion] SET [PredicateSerialized] = null
                        UPDATE [Promotion] SET [RewardsSerialized] = null
						UPDATE [DynamicContentPublishingGroup] SET [PredicateVisualTreeSerialized] = REPLACE([PredicateVisualTreeSerialized], 'ConditionExpressionTree', 'DynamicContentConditionTree')
                        UPDATE [DynamicContentPublishingGroup] SET [ConditionExpression] = null

                        BEGIN
                            DECLARE @typeInd varchar(10) = '""$type"":""';
                            DECLARE @commaInd varchar(10) = '"",';
                            DECLARE @index INT = 1;

                            WHILE @index < 20
                            BEGIN
                                UPDATE [Promotion] SET[PredicateVisualTreeSerialized] = REPLACE([PredicateVisualTreeSerialized], SUBSTRING([PredicateVisualTreeSerialized], CHARINDEX(@typeInd, [PredicateVisualTreeSerialized]), CHARINDEX(@commaInd, [PredicateVisualTreeSerialized], CHARINDEX(@typeInd, [PredicateVisualTreeSerialized])) - CHARINDEX(@typeInd, [PredicateVisualTreeSerialized]) + 2), '')
                                WHERE CHARINDEX(@typeInd, [PredicateVisualTreeSerialized]) > 0
                                SET @index = @index + 1;
                            END

                            SET @index = 1;
                            WHILE @index < 20
                            BEGIN
                                UPDATE [DynamicContentPublishingGroup] SET[PredicateVisualTreeSerialized] = REPLACE([PredicateVisualTreeSerialized], SUBSTRING([PredicateVisualTreeSerialized], CHARINDEX(@typeInd, [PredicateVisualTreeSerialized]), CHARINDEX(@commaInd, [PredicateVisualTreeSerialized], CHARINDEX(@typeInd, [PredicateVisualTreeSerialized])) - CHARINDEX(@typeInd, [PredicateVisualTreeSerialized]) + 2), '')
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
