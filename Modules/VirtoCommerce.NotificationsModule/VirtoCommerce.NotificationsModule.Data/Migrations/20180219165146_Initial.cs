using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

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
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    From = table.Column<string>(maxLength: 128, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Kind = table.Column<string>(maxLength: 128, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Number = table.Column<string>(maxLength: 128, nullable: true),
                    TenantId = table.Column<string>(maxLength: 128, nullable: true),
                    TenantType = table.Column<string>(maxLength: 128, nullable: true),
                    To = table.Column<string>(maxLength: 128, nullable: true),
                    Type = table.Column<string>(maxLength: 128, nullable: true)
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
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    FileName = table.Column<string>(maxLength: 512, nullable: true),
                    LanguageCode = table.Column<string>(maxLength: 10, nullable: true),
                    MimeType = table.Column<string>(maxLength: 50, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    NotificationEntityId = table.Column<string>(nullable: true),
                    Size = table.Column<string>(maxLength: 128, nullable: true),
                    Url = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationEmailAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationEmailAttachment_Notification_NotificationEntityId",
                        column: x => x.NotificationEntityId,
                        principalTable: "Notification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationEmailRecipient",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    EmailAddress = table.Column<string>(maxLength: 128, nullable: true),
                    NotificationId = table.Column<string>(nullable: true),
                    RecipientType = table.Column<int>(nullable: false)
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
                    Body = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LanguageCode = table.Column<string>(maxLength: 10, nullable: true),
                    LastSendAttemptDate = table.Column<DateTime>(nullable: true),
                    LastSendError = table.Column<string>(nullable: true),
                    MaxSendAttemptCount = table.Column<int>(nullable: false),
                    Message = table.Column<string>(maxLength: 1600, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    NotificationId = table.Column<string>(maxLength: 128, nullable: true),
                    NotificationType = table.Column<string>(maxLength: 128, nullable: true),
                    SendAttemptCount = table.Column<int>(nullable: false),
                    SendDate = table.Column<DateTime>(nullable: true),
                    Subject = table.Column<string>(maxLength: 512, nullable: true),
                    TenantId = table.Column<string>(maxLength: 128, nullable: true),
                    TenantType = table.Column<string>(maxLength: 128, nullable: true)
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
                    Body = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LanguageCode = table.Column<string>(maxLength: 10, nullable: true),
                    Message = table.Column<string>(maxLength: 1600, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    NotificationEntityId = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationTemplate_Notification_NotificationEntityId",
                        column: x => x.NotificationEntityId,
                        principalTable: "Notification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationEmailAttachment_NotificationEntityId",
                table: "NotificationEmailAttachment",
                column: "NotificationEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationEmailRecipient_NotificationId",
                table: "NotificationEmailRecipient",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationMessage_NotificationId",
                table: "NotificationMessage",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplate_NotificationEntityId",
                table: "NotificationTemplate",
                column: "NotificationEntityId");
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
