namespace VirtoCommerce.SubscriptionModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOuterId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subscription", "OuterId", c => c.String(maxLength: 256));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Subscription", "OuterId");
        }
    }
}
