ALTER TABLE [AspNetRoles] ADD [NormalizedName] nvarchar(256);
ALTER TABLE [AspNetRoles] ADD [ConcurrencyStamp] nvarchar(max);
ALTER TABLE [AspNetRoles] ADD [Description] nvarchar(max);
GO

DROP INDEX AspNetRoles.RoleNameIndex;
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles] ([NormalizedName]) WHERE ([NormalizedName] IS NOT NULL)
GO

ALTER TABLE [AspNetUserLogins] ADD [ProviderDisplayNames] nvarchar(max);
GO

ALTER TABLE [AspNetUsers] ADD [NormalizedUserName] nvarchar(256);
ALTER TABLE [AspNetUsers] ADD [NormalizedEmail] nvarchar(256);
ALTER TABLE [AspNetUsers] ADD [ConcurrencyStamp] nvarchar(max);
ALTER TABLE [AspNetUsers] ADD [LockoutEnd] datetimeoffset;
ALTER TABLE [AspNetUsers] ADD [StoreId] nvarchar(128);
ALTER TABLE [AspNetUsers] ADD [MemberId] nvarchar(128);
ALTER TABLE [AspNetUsers] ADD [IsAdministrator] BIT NOT NULL;
ALTER TABLE [AspNetUsers] ADD [PhotoUrl] nvarchar(2048);
ALTER TABLE [AspNetUsers] ADD [UserType] nvarchar(64);
ALTER TABLE [AspNetUsers] ADD [PasswordExpired] bit NOT NULL;
GO

CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]

CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims] ([RoleId])
GO

CREATE TABLE [dbo].[OpenIddictApplications](
	[ClientId] [nvarchar](450) NOT NULL,
	[ClientSecret] [nvarchar](max) NULL,
	[ConcurrencyToken] [nvarchar](max) NULL,
	[ConsentType] [nvarchar](max) NULL,
	[DisplayName] [nvarchar](max) NULL,
	[Id] [nvarchar](450) NOT NULL,
	[Permissions] [nvarchar](max) NULL,
	[PostLogoutRedirectUris] [nvarchar](max) NULL,
	[Properties] [nvarchar](max) NULL,
	[RedirectUris] [nvarchar](max) NULL,
	[Type] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_OpenIddictApplications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[OpenIddictAuthorizations](
	[ApplicationId] [nvarchar](450) NULL,
	[ConcurrencyToken] [nvarchar](max) NULL,
	[Id] [nvarchar](450) NOT NULL,
	[Properties] [nvarchar](max) NULL,
	[Scopes] [nvarchar](max) NULL,
	[Status] [nvarchar](max) NOT NULL,
	[Subject] [nvarchar](max) NOT NULL,
	[Type] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_OpenIddictAuthorizations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[OpenIddictAuthorizations]  WITH CHECK ADD  CONSTRAINT [FK_OpenIddictAuthorizations_OpenIddictApplications_ApplicationId] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[OpenIddictApplications] ([Id])
GO

ALTER TABLE [dbo].[OpenIddictAuthorizations] CHECK CONSTRAINT [FK_OpenIddictAuthorizations_OpenIddictApplications_ApplicationId]
GO

CREATE TABLE [dbo].[OpenIddictScopes](
	[ConcurrencyToken] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayName] [nvarchar](max) NULL,
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Properties] [nvarchar](max) NULL,
	[Resources] [nvarchar](max) NULL,
 CONSTRAINT [PK_OpenIddictScopes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[OpenIddictTokens](
	[ApplicationId] [nvarchar](450) NULL,
	[AuthorizationId] [nvarchar](450) NULL,
	[CreationDate] [datetimeoffset](7) NULL,
	[ExpirationDate] [datetimeoffset](7) NULL,
	[ConcurrencyToken] [nvarchar](max) NULL,
	[Id] [nvarchar](450) NOT NULL,
	[Payload] [nvarchar](max) NULL,
	[Properties] [nvarchar](max) NULL,
	[ReferenceId] [nvarchar](450) NULL,
	[Status] [nvarchar](max) NULL,
	[Subject] [nvarchar](max) NOT NULL,
	[Type] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_OpenIddictTokens] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[OpenIddictTokens]  WITH CHECK ADD  CONSTRAINT [FK_OpenIddictTokens_OpenIddictApplications_ApplicationId] FOREIGN KEY([ApplicationId])
REFERENCES [dbo].[OpenIddictApplications] ([Id])
GO

ALTER TABLE [dbo].[OpenIddictTokens] CHECK CONSTRAINT [FK_OpenIddictTokens_OpenIddictApplications_ApplicationId]
GO

ALTER TABLE [dbo].[OpenIddictTokens]  WITH CHECK ADD  CONSTRAINT [FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId] FOREIGN KEY([AuthorizationId])
REFERENCES [dbo].[OpenIddictAuthorizations] ([Id])
GO

ALTER TABLE [dbo].[OpenIddictTokens] CHECK CONSTRAINT [FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId]
GO



CREATE TABLE [dbo].[__EFMigrationsHistory]( [MigrationId] [nvarchar](150) NOT NULL, [ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED ( [MigrationId] ASC )
 WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]
GO

INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20180619060741_Initial', '2.1.8-servicing-32085')
INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20181102114047_AddPasswordExpired', '2.1.8-servicing-32085')
GO

INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20180213153553_Initial', '2.1.8-servicing-32085')
INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20180525101112_AssetsEntity', '2.1.8-servicing-32085')
INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20180531115304_UpDecimalPrecisionToFive', '2.1.8-servicing-32085')
INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20180709123002_SettingsCleanup', '2.1.8-servicing-32085')
GO

UPDATE [AspNetUsers] SET SecurityStamp = 'E5STZEUPBSKSY35N74WEILHM7U56RM5T' where Id = '1eb2fa8ac6574541afdb525833dadb46'
UPDATE [AspNetUsers] SET ConcurrencyStamp = 'b9523e0f-830b-4f8f-95a0-5e12e7262f88' where Id = '1eb2fa8ac6574541afdb525833dadb46'
UPDATE [AspNetUsers] SET NormalizedUserName = 'ADMIN' where Id = '1eb2fa8ac6574541afdb525833dadb46'
INSERT INTO [dbo].[AspNetUserRoles] ([UserId],[RoleId]) VALUES ('1eb2fa8ac6574541afdb525833dadb46','894f3c74-cadb-47c0-8cdd-c738526aab47')
GO


