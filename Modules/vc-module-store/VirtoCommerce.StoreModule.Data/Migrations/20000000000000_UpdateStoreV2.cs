using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.StoreModule.Data.Migrations
{
    public partial class UpdateStoreV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '__MigrationHistory'))
                 BEGIN
                    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20190522054351_InitialStore', '2.2.3-servicing-35854')
                    UPDATE [PlatformDynamicProperty] SET ObjectType = 'VirtoCommerce.StoreModule.Core.Model.Store' WHERE ObjectType = 'VirtoCommerce.Domain.Store.Model.Store'
                    UPDATE [PlatformDynamicPropertyObjectValue] SET ObjectType = 'VirtoCommerce.StoreModule.Core.Model.Store' WHERE ObjectType = 'VirtoCommerce.Domain.Store.Model.Store'
                END");

            migrationBuilder.Sql(@"IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '__MigrationHistory'))
                    BEGIN
                        CREATE TABLE [dbo].[StoreSeoInfo](
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
                         CONSTRAINT [PK_StoreSeoInfo] PRIMARY KEY CLUSTERED 
                        (
                            [Id] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                        ) ON [PRIMARY]

                        BEGIN
                            ALTER TABLE [dbo].[StoreSeoInfo]  WITH CHECK ADD  CONSTRAINT [FK_StoreSeoInfo_Store_StoreId] FOREIGN KEY([StoreId])
                            REFERENCES [dbo].[Store] ([Id]) ON DELETE CASCADE
                            ALTER TABLE [dbo].[StoreSeoInfo] CHECK CONSTRAINT [FK_StoreSeoInfo_Store_StoreId]
                        END
                        
                    END");

            migrationBuilder.Sql(@"IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '__MigrationHistory'))
                    BEGIN
                        INSERT INTO [StoreSeoInfo] ([Id], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy], [Keyword], [StoreId], [IsActive], [Language], [Title], [MetaDescription], [MetaKeywords], [ImageAltDescription])
                          SELECT [Id], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy], [Keyword], [ObjectId] as [StoreId], [IsActive], [Language], [Title], [MetaDescription], [MetaKeywords], [ImageAltDescription]  FROM [SeoUrlKeyword] WHERE ObjectType = 'Store'              
                    END");

            migrationBuilder.Sql(@"IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '__MigrationHistory'))
                    BEGIN
                        CREATE TABLE [dbo].[StoreDynamicPropertyObjectValue](
	                        [Id] [nvarchar](128) NOT NULL,
	                        [CreatedDate] [datetime2](7) NOT NULL,
	                        [ModifiedDate] [datetime2](7) NULL,
	                        [CreatedBy] [nvarchar](64) NULL,
	                        [ModifiedBy] [nvarchar](64) NULL,
	                        [ObjectType] [nvarchar](256) NULL,
	                        [ObjectId] [nvarchar](128) NULL,
	                        [Locale] [nvarchar](64) NULL,
	                        [ValueType] [nvarchar](64) NOT NULL,
	                        [ShortTextValue] [nvarchar](512) NULL,
	                        [LongTextValue] [nvarchar](max) NULL,
	                        [DecimalValue] [decimal](18, 5) NULL,
	                        [IntegerValue] [int] NULL,
	                        [BooleanValue] [bit] NULL,
	                        [DateTimeValue] [datetime2](7) NULL,
	                        [PropertyId] [nvarchar](max) NULL,
	                        [DictionaryItemId] [nvarchar](max) NULL,
                         CONSTRAINT [PK_StoreDynamicPropertyObjectValue] PRIMARY KEY CLUSTERED 
                        (
	                        [Id] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

                        BEGIN
                            ALTER TABLE [dbo].[StoreDynamicPropertyObjectValue]  WITH CHECK ADD  CONSTRAINT [FK_StoreDynamicPropertyObjectValue_Store_ObjectId] FOREIGN KEY([ObjectId])
                            REFERENCES [dbo].[Store] ([Id])
                            ON DELETE CASCADE
                            ALTER TABLE [dbo].[StoreDynamicPropertyObjectValue] CHECK CONSTRAINT [FK_StoreDynamicPropertyObjectValue_Store_ObjectId]
                        END

                        BEGIN
                            CREATE NONCLUSTERED INDEX [IX_ObjectType_ObjectId] ON [dbo].[StoreDynamicPropertyObjectValue] ([ObjectType] ASC,[ObjectId] ASC)
                                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                        END

                    END");

            migrationBuilder.Sql(@"IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '__MigrationHistory'))
                    BEGIN
                        INSERT INTO [dbo].[StoreDynamicPropertyObjectValue] ([Id],[CreatedDate],[ModifiedDate],[CreatedBy],[ModifiedBy],[ObjectType],[ObjectId],[Locale],[ValueType],[ShortTextValue],[LongTextValue],[DecimalValue],[IntegerValue],[BooleanValue],[DateTimeValue],[PropertyId],[DictionaryItemId])
                        SELECT [Id],[CreatedDate],[ModifiedDate],[CreatedBy],[ModifiedBy],[ObjectType],[ObjectId],[Locale],[ValueType],[ShortTextValue],[LongTextValue],[DecimalValue],[IntegerValue],[BooleanValue],[DateTimeValue],[PropertyId],[DictionaryItemId]
                        FROM [PlatformDynamicPropertyObjectValue]
                        WHERE ObjectType = 'VirtoCommerce.StoreModule.Core.Model.Store'
                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
