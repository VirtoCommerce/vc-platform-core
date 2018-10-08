namespace VirtoCommerce.SubscriptionModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddComment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subscription", "Comment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Subscription", "Comment");
        }
    }
}
