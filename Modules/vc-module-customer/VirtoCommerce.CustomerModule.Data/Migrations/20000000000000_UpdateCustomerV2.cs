using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.CustomerModule.Data.Migrations
{
    public partial class UpdateCustomerV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__MigrationHistory'))
                IF (EXISTS (SELECT * FROM __MigrationHistory WHERE ContextKey = 'VirtoCommerce.CustomerModule.Data.Migrations.Configuration'))
                    BEGIN

	                    BEGIN
		                    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20190510074541_InitialCustomer', '2.2.3-servicing-35854')
	                    END
	                    
	                    BEGIN
		                    ALTER TABLE [Member] ADD [Discriminator] nvarchar(128) NOT NULL DEFAULT ('Member')
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
                            ALTER TABLE [Member] ADD [Description] nvarchar(max) NULL
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

                        BEGIN
                            UPDATE [PlatformDynamicPropertyObjectValue] SET ObjectType = 'VirtoCommerce.CustomerModule.Core.Model.Contact' WHERE ObjectType = 'VirtoCommerce.Domain.Customer.Model.Contact'
                            UPDATE [PlatformDynamicPropertyObjectValue] SET ObjectType = 'VirtoCommerce.CustomerModule.Core.Model.Organization' WHERE ObjectType = 'VirtoCommerce.Domain.Customer.Model.Organization'
                            UPDATE [PlatformDynamicPropertyObjectValue] SET ObjectType = 'VirtoCommerce.CustomerModule.Core.Model.Employee' WHERE ObjectType = 'VirtoCommerce.Domain.Customer.Model.Employee'
                            UPDATE [PlatformDynamicPropertyObjectValue] SET ObjectType = 'VirtoCommerce.CustomerModule.Core.Model.Vendor' WHERE ObjectType = 'VirtoCommerce.Domain.Customer.Model.Vendor'
                        END
            END");

            migrationBuilder.Sql(@"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__MigrationHistory'))
                IF (EXISTS (SELECT * FROM __MigrationHistory WHERE ContextKey = 'VirtoCommerce.CustomerModule.Data.Migrations.Configuration'))
                    BEGIN
                        CREATE TABLE [dbo].[MemberSeoInfo](
	                        [Id] [nvarchar](128) NOT NULL,
	                        [CreatedDate] [datetime2](7) NOT NULL,
	                        [ModifiedDate] [datetime2](7) NULL,
	                        [CreatedBy] [nvarchar](64) NULL,
	                        [ModifiedBy] [nvarchar](64) NULL,
	                        [Keyword] [nvarchar](255) NOT NULL,
	                        [StoreId] [nvarchar](128) NULL,
	                        [IsActive] [bit] NOT NULL,
	                        [Language] [nvarchar](5) NULL,
	                        [Title] [nvarchar](255) NULL,
	                        [MetaDescription] [nvarchar](1024) NULL,
	                        [MetaKeywords] [nvarchar](255) NULL,
	                        [ImageAltDescription] [nvarchar](255) NULL,
	                        [MemberId] [nvarchar](128) NULL,
                         CONSTRAINT [PK_MemberSeoInfo] PRIMARY KEY CLUSTERED 
                        (
	                        [Id] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                        ) ON [PRIMARY]
                        
                        BEGIN
                            ALTER TABLE [dbo].[MemberSeoInfo]  WITH CHECK ADD  CONSTRAINT [FK_MemberSeoInfo_Member_MemberId] FOREIGN KEY([MemberId])
                                REFERENCES [dbo].[Member] ([Id]) ON DELETE CASCADE
                            ALTER TABLE [dbo].[MemberSeoInfo] CHECK CONSTRAINT [FK_MemberSeoInfo_Member_MemberId]
                        END
                    END");

            migrationBuilder.Sql(@"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__MigrationHistory'))
                IF (EXISTS (SELECT * FROM __MigrationHistory WHERE ContextKey = 'VirtoCommerce.CustomerModule.Data.Migrations.Configuration'))
                    BEGIN
                        INSERT INTO [MemberSeoInfo] ([Id], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy], [Keyword], [StoreId], [IsActive], [Language], [Title], [MetaDescription], [MetaKeywords], [ImageAltDescription], [MemberId])
                              SELECT [Id], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy], [Keyword], [StoreId], [IsActive], [Language], [Title], [MetaDescription], [MetaKeywords], [ImageAltDescription], [ObjectId] as [MemberId]  FROM [SeoUrlKeyword] WHERE ObjectType = 'Vendor'
				    END");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
