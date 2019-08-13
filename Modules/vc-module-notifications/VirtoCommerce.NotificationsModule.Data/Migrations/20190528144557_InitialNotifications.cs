using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.NotificationsModule.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    TenantId = table.Column<string>(maxLength: 128, nullable: true),
                    TenantType = table.Column<string>(maxLength: 128, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Type = table.Column<string>(maxLength: 128, nullable: true),
                    Kind = table.Column<string>(maxLength: 128, nullable: true),
                    Discriminator = table.Column<string>(maxLength: 128, nullable: false),
                    From = table.Column<string>(maxLength: 128, nullable: true),
                    To = table.Column<string>(maxLength: 128, nullable: true),
                    Number = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationEmailAttachment",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    FileName = table.Column<string>(maxLength: 512, nullable: true),
                    Url = table.Column<string>(maxLength: 2048, nullable: true),
                    MimeType = table.Column<string>(maxLength: 50, nullable: true),
                    Size = table.Column<string>(maxLength: 128, nullable: true),
                    LanguageCode = table.Column<string>(maxLength: 10, nullable: true),
                    NotificationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationEmailAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationEmailAttachment_Notification_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationEmailRecipient",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    EmailAddress = table.Column<string>(maxLength: 128, nullable: true),
                    RecipientType = table.Column<int>(nullable: false),
                    NotificationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationEmailRecipient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationEmailRecipient_Notification_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationMessage",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    TenantId = table.Column<string>(maxLength: 128, nullable: true),
                    TenantType = table.Column<string>(maxLength: 128, nullable: true),
                    NotificationType = table.Column<string>(maxLength: 128, nullable: true),
                    SendAttemptCount = table.Column<int>(nullable: false),
                    MaxSendAttemptCount = table.Column<int>(nullable: false),
                    LastSendError = table.Column<string>(nullable: true),
                    LastSendAttemptDate = table.Column<DateTime>(nullable: true),
                    SendDate = table.Column<DateTime>(nullable: true),
                    LanguageCode = table.Column<string>(maxLength: 10, nullable: true),
                    NotificationId = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(maxLength: 128, nullable: false),
                    Subject = table.Column<string>(maxLength: 512, nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Message = table.Column<string>(maxLength: 1600, nullable: true),
                    Number = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationMessage_Notification_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTemplate",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    LanguageCode = table.Column<string>(maxLength: 10, nullable: true),
                    NotificationId = table.Column<string>(maxLength: 128, nullable: true),
                    Discriminator = table.Column<string>(maxLength: 128, nullable: false),
                    Subject = table.Column<string>(maxLength: 512, nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Message = table.Column<string>(maxLength: 1600, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationTemplate_Notification_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationEmailAttachment_NotificationId",
                table: "NotificationEmailAttachment",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationEmailRecipient_NotificationId",
                table: "NotificationEmailRecipient",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationMessage_NotificationId",
                table: "NotificationMessage",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplate_NotificationId",
                table: "NotificationTemplate",
                column: "NotificationId");


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
            migrationBuilder.DropTable(
                name: "NotificationEmailAttachment");

            migrationBuilder.DropTable(
                name: "NotificationEmailRecipient");

            migrationBuilder.DropTable(
                name: "NotificationMessage");

            migrationBuilder.DropTable(
                name: "NotificationTemplate");

            migrationBuilder.DropTable(
                name: "Notification");
        }
    }
}
