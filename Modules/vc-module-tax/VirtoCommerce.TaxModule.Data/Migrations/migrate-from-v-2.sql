INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20190409093711_InitialTax', '2.2.3-servicing-35854')

ALTER TABLE [StoreTaxProvider] ADD [TypeName] nvarchar(128) NOT NULL Default ('')
