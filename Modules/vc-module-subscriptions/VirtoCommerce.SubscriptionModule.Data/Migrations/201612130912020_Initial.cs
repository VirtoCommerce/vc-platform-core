namespace VirtoCommerce.SubscriptionModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Subscription",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        StoreId = c.String(nullable: false, maxLength: 64),
                        CustomerId = c.String(nullable: false, maxLength: 64),
                        CustomerName = c.String(maxLength: 255),
                        Number = c.String(nullable: false, maxLength: 64),
                        Balance = c.Decimal(nullable: false, storeType: "money"),
                        Interval = c.String(maxLength: 64),
                        IntervalCount = c.Int(nullable: false),
                        TrialPeriodDays = c.Int(nullable: false),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        TrialSart = c.DateTime(),
                        TrialEnd = c.DateTime(),
                        CurrentPeriodStart = c.DateTime(),
                        CurrentPeriodEnd = c.DateTime(),
                        Status = c.String(maxLength: 64),
                        IsCancelled = c.Boolean(nullable: false),
                        CancelledDate = c.DateTime(),
                        CancelReason = c.String(maxLength: 2048),
                        CustomerOrderPrototypeId = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedBy = c.String(maxLength: 64),
                        ModifiedBy = c.String(maxLength: 64),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PaymentPlan",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Interval = c.String(maxLength: 64),
                        IntervalCount = c.Int(nullable: false),
                        TrialPeriodDays = c.Int(nullable: false),
                        ProductId = c.String(maxLength: 128),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        CreatedBy = c.String(maxLength: 64),
                        ModifiedBy = c.String(maxLength: 64),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PaymentPlan");
            DropTable("dbo.Subscription");
        }
    }
}
