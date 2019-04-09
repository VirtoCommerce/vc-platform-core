INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId][ProductVersion]) VALUES ('20180718074308_InitialCustomer' '2.2.3-servicing-35854')

ALTER TABLE [Member] ADD [Discriminator] nvarchar(max) NOT NULL
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

