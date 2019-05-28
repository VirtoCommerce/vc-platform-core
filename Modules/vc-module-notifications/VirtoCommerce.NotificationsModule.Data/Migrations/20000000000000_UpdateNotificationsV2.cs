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
                    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES ('20190528144557_Initial', '2.2.3-servicing-35854')
                END");

            migrationBuilder.Sql(@"IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '__MigrationHistory'))
                    BEGIN
                        BEGIN
	                        INSERT INTO [dbo].[Notification]
                                       ([Id]
                                       ,[CreatedDate]
                                       ,[ModifiedDate]
                                       ,[CreatedBy]
                                       ,[ModifiedBy]
                                       ,[TenantId]
                                       ,[TenantType]
                                       ,[IsActive]
                                       ,[Type]
                                       ,[Kind]
                                       ,[Discriminator]
                                       ,[From]
                                       ,[To])
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
	                            null, null
                            FROM [PlatformNotificationTemplate] pnt
                            GROUP BY [NotificationTypeId]
                        END

                        BEGIN
                            INSERT INTO [dbo].[NotificationTemplate]
                                       ([Id]
                                       ,[CreatedDate]
                                       ,[ModifiedDate]
                                       ,[CreatedBy]
                                       ,[ModifiedBy]
                                       ,[LanguageCode]
                                       ,[Subject]
                                       ,[Body]
                                       ,[Message]
                                       ,[NotificationId]
                                       ,[Discriminator])
                            SELECT [Id]
                                  ,[CreatedDate]
                                  ,[ModifiedDate]
                                  ,[CreatedBy]
                                  ,[ModifiedBy]
	                              ,[Language]
	                              , [Subject]
                                  , CASE WHEN [NotificationTypeId] LIKE '%EmailNotification%' THEN [Body] ELSE '' END
	                              , CASE WHEN [NotificationTypeId] LIKE '%SmsNotification%' THEN [Body] ELSE '' END
                                  , (SELECT TOP 1 n.Id FROM [dbo].[Notification] n WHERE n.[Type] = pnt.[NotificationTypeId])
                                  , CASE WHEN [NotificationTypeId] LIKE '%EmailNotification%' THEN 'EmailNotificationTemplateEntity' ELSE 'SmsNotificationTemplateEntity' END
                              FROM [PlatformNotificationTemplate] pnt
                        END

                        BEGIN
                            INSERT INTO [dbo].[NotificationMessage]
                                       ([Id]
                                       ,[CreatedDate]
                                       ,[ModifiedDate]
                                       ,[CreatedBy]
                                       ,[ModifiedBy]
                                       ,[TenantId]
                                       ,[TenantType]
                                       ,[NotificationId]
                                       ,[NotificationType]
                                       ,[SendAttemptCount]
                                       ,[MaxSendAttemptCount]
                                       ,[LastSendError]
                                       ,[LastSendAttemptDate]
                                       ,[SendDate]
                                       ,[LanguageCode]
                                       ,[Subject]
                                       ,[Body]
                                       ,[Message],
                                        [Discriminator])
                            SELECT [Id]
	                              ,[CreatedDate]
                                  ,[ModifiedDate]
                                  ,[CreatedBy]
                                  ,[ModifiedBy]	
	                              ,[ObjectId]
                                  ,[ObjectTypeId]
                                  , (SELECT TOP 1 n.Id FROM [dbo].[Notification] n WHERE n.[Type] = pn.[Type])
	                              , [Type]
                                  ,[AttemptCount]
	                              ,[MaxAttemptCount]
	                              ,[LastFailAttemptMessage]
                                  ,[LastFailAttemptDate]
	                              ,[SentDate]
	                              ,[Language]
	                              ,[Subject]
                                  , CASE WHEN pn.[Type] LIKE '%EmailNotification%' THEN [Body] ELSE '' END
	                              , CASE WHEN pn.[Type] LIKE '%SmsNotification%' THEN [Body] ELSE '' END
                                  , CASE WHEN [Type] LIKE '%EmailNotification%' THEN 'EmailNotificationMessageEntity' ELSE 'SmsNotificationMessageEntity' END
                              FROM [PlatformNotification] pn
                        END
                        
                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
