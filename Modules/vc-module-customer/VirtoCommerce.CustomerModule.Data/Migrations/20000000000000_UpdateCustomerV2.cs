using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.CustomerModule.Data.Migrations
{
    public partial class UpdateCustomerV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '__MigrationHistory'))
                    BEGIN

	                    BEGIN
		                    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20190510074541_InitialCustomer', '2.2.3-servicing-35854')
	                    END
	                    
	                    BEGIN
		                    ALTER TABLE [Member] ADD [Discriminator] nvarchar(max) NOT NULL DEFAULT ('Member')
                            ALTER TABLE [Member] ADD [FirstName] nvarchar(128) NULL
                            ALTER TABLE [Member] ADD [MiddleName] nvarchar(128) NULL
                            ALTER TABLE [Member] ADD [LastName] nvarchar(128) NULL
                            ALTER TABLE [Member] ADD [FullName] nvarchar(254) NULL
                            ALTER TABLE [Member] ADD [TimeZone] nvarchar(32) NULL
                            ALTER TABLE [Member] ADD [DefaultLanguage] nvarchar(32) NULL
                            ALTER TABLE [Member] ADD [BirthDate] [datetime2] NULL
                            ALTER TABLE [Member] ADD [TaxpayerId] nvarchar(64) NULL
                            ALTER TABLE [Member] ADD [PreferredDelivery] nvarchar(64) NULL
                            ALTER TABLE [Member] ADD [PreferredCommunication] nvarchar(64) NULL
                            ALTER TABLE [Member] ADD [PhotoUrl] nvarchar(2083) NULL
                            ALTER TABLE [Member] ADD [Salutation] nvarchar(256) NULL
                            ALTER TABLE [Member] ADD [Type] nvarchar(64) NULL
                            ALTER TABLE [Member] ADD [IsActive] [bit] NULL
                            ALTER TABLE [Member] ADD [Description] nvarchar(256) NULL
                            ALTER TABLE [Member] ADD [BusinessCategory] nvarchar(64) NULL
                            ALTER TABLE [Member] ADD [OwnerId] nvarchar(128) NULL
                            ALTER TABLE [Member] ADD [SiteUrl] nvarchar(2048) NULL
                            ALTER TABLE [Member] ADD [LogoUrl] nvarchar(2048) NULL
                            ALTER TABLE [Member] ADD [GroupName] nvarchar(64) NULL
	                    END

	                    BEGIN
		                    EXEC (N'UPDATE Member SET Discriminator = CONCAT(MemberType, ''Entity'') , FirstName = c.FirstName,
                                    MiddleName = c.MiddleName, LastName = c.LastName, FullName = c.FullName, TimeZone = c.TimeZone,
	                                DefaultLanguage = c.DefaultLanguage, BirthDate = c.BirthDate, TaxpayerId = c.TaxpayerId,
                                    PreferredDelivery = c.PreferredDelivery, PreferredCommunication = c.PreferredCommunication, 
	                                PhotoUrl = c.PhotoUrl, Salutation = c.Salutation
                                    FROM Member m INNER JOIN Contact c ON c.Id = m.Id')
		                    EXEC (N'UPDATE Member SET Discriminator = CONCAT(MemberType, ''Entity'') , [Type] = o.OrgType,
                                    [Description] = o.Description, BusinessCategory = o.BusinessCategory, OwnerId = o.OwnerId
                                    FROM Member m INNER JOIN Organization o ON o.Id = m.Id')
		                    EXEC (N'UPDATE Member SET Discriminator = CONCAT(MemberType, ''Entity'') , [Description] = v.Description,
                                    SiteUrl = v.SiteUrl, LogoUrl = v.LogoUrl, GroupName = v.GroupName
                                    FROM Member m INNER JOIN Vendor v ON v.Id = m.Id')
                            EXEC (N'UPDATE Member SET Discriminator = CONCAT(MemberType, ''Entity'') , [Type] = e.[Type], FirstName = e.FirstName,
                                    MiddleName = e.MiddleName, LastName = e.LastName, FullName = e.FullName, TimeZone = e.TimeZone,
	                                    DefaultLanguage = e.DefaultLanguage, BirthDate = e.BirthDate, PhotoUrl = e.PhotoUrl, IsActive = e.IsActive
                                    FROM Member m INNER JOIN Employee e ON e.Id = m.Id')
	                    END

                        BEGIN
                            UPDATE [PlatformDynamicProperty] SET ObjectType = 'VirtoCommerce.CustomerModule.Core.Model.Contact' WHERE ObjectType = 'VirtoCommerce.Domain.Customer.Model.Contact'
                            UPDATE [PlatformDynamicProperty] SET ObjectType = 'VirtoCommerce.CustomerModule.Core.Model.Organization' WHERE ObjectType = 'VirtoCommerce.Domain.Customer.Model.Organization'
                            UPDATE [PlatformDynamicProperty] SET ObjectType = 'VirtoCommerce.CustomerModule.Core.Model.Employee' WHERE ObjectType = 'VirtoCommerce.Domain.Customer.Model.Employee'
                            UPDATE [PlatformDynamicProperty] SET ObjectType = 'VirtoCommerce.CustomerModule.Core.Model.Vendor' WHERE ObjectType = 'VirtoCommerce.Domain.Customer.Model.Vendor'
                        END
            END");


            migrationBuilder.CreateTable(
              name: "MemberSeoInfo",
              columns: table => new
              {
                  Id = table.Column<string>(maxLength: 128, nullable: false),
                  CreatedDate = table.Column<DateTime>(nullable: false),
                  ModifiedDate = table.Column<DateTime>(nullable: true),
                  CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                  ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                  Keyword = table.Column<string>(maxLength: 255, nullable: false),
                  StoreId = table.Column<string>(maxLength: 128, nullable: true),
                  IsActive = table.Column<bool>(nullable: false),
                  Language = table.Column<string>(maxLength: 5, nullable: true),
                  Title = table.Column<string>(maxLength: 255, nullable: true),
                  MetaDescription = table.Column<string>(maxLength: 1024, nullable: true),
                  MetaKeywords = table.Column<string>(maxLength: 255, nullable: true),
                  ImageAltDescription = table.Column<string>(maxLength: 255, nullable: true),
                  MemberId = table.Column<string>(nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_MemberSeoInfo", x => x.Id);
                  table.ForeignKey(
                      name: "FK_MemberSeoInfo_Member_MemberId",
                      column: x => x.MemberId,
                      principalTable: "Member",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
              });

            migrationBuilder.CreateIndex(
                name: "IX_MemberSeoInfo_MemberId",
                table: "MemberSeoInfo",
                column: "MemberId");

            migrationBuilder.Sql(@" INSERT INTO [MemberSeoInfo] ([Id], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy], [Keyword], [StoreId], [IsActive], [Language], [Title], [MetaDescription], [MetaKeywords], [ImageAltDescription], [MemberId])
                              SELECT [Id], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy], [Keyword], [StoreId], [IsActive], [Language], [Title], [MetaDescription], [MetaKeywords], [ImageAltDescription], [ObjectId] as [MemberId]  FROM [SeoUrlKeyword] WHERE ObjectType = 'Vendor'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
