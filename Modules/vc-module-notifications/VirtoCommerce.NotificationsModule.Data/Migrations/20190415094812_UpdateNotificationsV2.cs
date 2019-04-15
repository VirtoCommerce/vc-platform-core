using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.NotificationsModule.Data.Migrations
{
    public partial class UpdateNotificationsV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '__MigrationHistory'))
                    BEGIN
	                    SELECT NEWID(), 
	                        (SELECT TOP 1 pnt2.CreatedDate FROM [PlatformNotificationTemplate] pnt2 WHERE pnt2.NotificationTypeId = pnt.NotificationTypeId), 
	                        (SELECT TOP 1 pnt2.ModifiedDate FROM [PlatformNotificationTemplate] pnt2 WHERE pnt2.NotificationTypeId = pnt.NotificationTypeId), 
	                        (SELECT TOP 1 pnt2.CreatedBy FROM [PlatformNotificationTemplate] pnt2 WHERE pnt2.NotificationTypeId = pnt.NotificationTypeId), 
	                        (SELECT TOP 1 pnt2.ModifiedBy FROM [PlatformNotificationTemplate] pnt2 WHERE pnt2.NotificationTypeId = pnt.NotificationTypeId), 
	                        (SELECT TOP 1 pnt2.ObjectId FROM [PlatformNotificationTemplate] pnt2 WHERE pnt2.NotificationTypeId = pnt.NotificationTypeId), 
	                        (SELECT TOP 1 pnt2.ObjectTypeId FROM [PlatformNotificationTemplate] pnt2 WHERE pnt2.NotificationTypeId = pnt.NotificationTypeId), 
	                        1, [NotificationTypeId], 
	                        CASE WHEN [NotificationTypeId] LIKE '%EmailNotification%' THEN 'EmailNotification' ELSE 'SmsNotification' END,
	                        CASE WHEN [NotificationTypeId] LIKE '%EmailNotification%' THEN 'EmailNotificationEntity' ELSE 'SmsNotificationEntity' END,
	                        null, null, null
                        FROM [VirtoCommerce2].[dbo].[PlatformNotificationTemplate] pnt
                        GROUP BY [NotificationTypeId]
                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
