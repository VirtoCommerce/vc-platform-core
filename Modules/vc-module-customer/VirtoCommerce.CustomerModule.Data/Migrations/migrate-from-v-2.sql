INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20180718074308_InitialCustomer', '2.2.3-servicing-35854')
GO

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
GO

UPDATE Member SET Discriminator = CONCAT(MemberType, 'Entity') , FirstName = c.FirstName, MiddleName = c.MiddleName, LastName = c.LastName, FullName = c.FullName, TimeZone = c.TimeZone,
	DefaultLanguage = c.DefaultLanguage, BirthDate = c.BirthDate, TaxpayerId = c.TaxpayerId, PreferredDelivery = c.PreferredDelivery, PreferredCommunication = c.PreferredCommunication, 
	PhotoUrl = c.PhotoUrl, Salutation = c.Salutation
FROM Member m INNER JOIN Contact c ON c.Id = m.Id

UPDATE Member SET Discriminator = CONCAT(MemberType, 'Entity') , [Type] = o.OrgType, [Description] = o.Description, BusinessCategory = o.BusinessCategory, OwnerId = o.OwnerId
FROM Member m INNER JOIN Organization o ON o.Id = m.Id

UPDATE Member SET Discriminator = CONCAT(MemberType, 'Entity') , [Description] = v.Description, SiteUrl = v.SiteUrl, LogoUrl = v.LogoUrl, GroupName = v.GroupName
FROM Member m INNER JOIN Vendor v ON v.Id = m.Id

UPDATE Member SET Discriminator = CONCAT(MemberType, 'Entity') , [Type] = e.[Type], FirstName = e.FirstName, MiddleName = e.MiddleName, LastName = e.LastName, FullName = e.FullName, TimeZone = e.TimeZone,
	DefaultLanguage = e.DefaultLanguage, BirthDate = e.BirthDate, PhotoUrl = e.PhotoUrl, IsActive = e.IsActive
FROM Member m INNER JOIN Employee e ON e.Id = m.Id




