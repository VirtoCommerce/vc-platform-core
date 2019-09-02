using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.StoreModule.Data.Migrations
{
    public partial class UpdateStoreV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__MigrationHistory'))
                IF (EXISTS (SELECT * FROM __MigrationHistory WHERE ContextKey = 'VirtoCommerce.StoreModule.Data.Migrations.Configuration'))
                 BEGIN
                    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES ('20190522054351_InitialStore', '2.2.3-servicing-35854')
                    UPDATE [PlatformDynamicProperty]            SET ObjectType = 'VirtoCommerce.StoreModule.Core.Model.Store' WHERE ObjectType = 'VirtoCommerce.Domain.Store.Model.Store'
                    UPDATE [PlatformDynamicPropertyObjectValue] SET ObjectType = 'VirtoCommerce.StoreModule.Core.Model.Store' WHERE ObjectType = 'VirtoCommerce.Domain.Store.Model.Store'
                END");

            migrationBuilder.Sql(@"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__MigrationHistory'))
                IF (EXISTS (SELECT * FROM __MigrationHistory WHERE ContextKey = 'VirtoCommerce.StoreModule.Data.Migrations.Configuration'))
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

            migrationBuilder.Sql(@"IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__MigrationHistory'))
                IF (EXISTS (SELECT * FROM __MigrationHistory WHERE ContextKey = 'VirtoCommerce.StoreModule.Data.Migrations.Configuration'))
                    BEGIN
                        INSERT INTO [StoreSeoInfo] ([Id], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy], [Keyword], [StoreId], [IsActive], [Language], [Title], [MetaDescription], [MetaKeywords], [ImageAltDescription])
                          SELECT [Id], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy], [Keyword], [ObjectId] as [StoreId], [IsActive], [Language], [Title], [MetaDescription], [MetaKeywords], [ImageAltDescription]  FROM [SeoUrlKeyword] WHERE ObjectType = 'Store'              
                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
