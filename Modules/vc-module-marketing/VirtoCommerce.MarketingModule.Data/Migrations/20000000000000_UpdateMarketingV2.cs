using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.MarketingModule.Data.Migrations
{
    public partial class UpdateMarketingV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '__MigrationHistory'))
                    BEGIN
	                    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20190409185706_InitialMarketing', '2.2.3-servicing-35854')

                        UPDATE [PlatformDynamicProperty] SET ObjectType = 'VirtoCommerce.MarketingModule.Core.Model.DynamicContentItem' WHERE [Name] = 'Content type' AND ObjectType = 'VirtoCommerce.Domain.Marketing.Model.DynamicContentItem'
                        UPDATE [PlatformDynamicProperty] SET ObjectType = 'VirtoCommerce.MarketingModule.Core.Model.DynamicContentItem' WHERE [Name] = 'Html' AND ObjectType = 'VirtoCommerce.Domain.Marketing.Model.DynamicContentItem'
                        UPDATE [Promotion] SET [PredicateVisualTreeSerialized] = REPLACE([PredicateVisualTreeSerialized], 'PromoDynamicExpressionTree', 'PromotionConditionAndRewardTree')
                        UPDATE [Promotion] SET [PredicateVisualTreeSerialized] = REPLACE([PredicateVisualTreeSerialized], 'RewardBlock', 'BlockReward')
                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
