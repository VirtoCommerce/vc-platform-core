// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using VirtoCommerce.CatalogModule.Data.Repositories;

namespace VirtoCommerce.CatalogModule.Data.Migrations
{
    [DbContext(typeof(CatalogDbContext))]
    [Migration("20000000000000_UpdateCatalogV2")]
    partial class UpdateCatalogV2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.AssetEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<string>("CreatedBy")
                    .HasMaxLength(64);

                b.Property<DateTime>("CreatedDate");

                b.Property<string>("ItemId")
                    .IsRequired();

                b.Property<string>("LanguageCode")
                    .HasMaxLength(5);

                b.Property<string>("MimeType")
                    .HasMaxLength(128);

                b.Property<string>("ModifiedBy")
                    .HasMaxLength(64);

                b.Property<DateTime?>("ModifiedDate");

                b.Property<string>("Name")
                    .HasMaxLength(1024);

                b.Property<long>("Size");

                b.Property<string>("Url")
                    .IsRequired()
                    .HasMaxLength(2083);

                b.HasKey("Id");

                b.HasIndex("ItemId");

                b.ToTable("CatalogAsset");
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.AssociationEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<string>("AssociatedCategoryId");

                b.Property<string>("AssociatedItemId");

                b.Property<string>("AssociationType")
                    .IsRequired()
                    .HasMaxLength(128);

                b.Property<string>("CreatedBy")
                    .HasMaxLength(64);

                b.Property<DateTime>("CreatedDate");

                b.Property<string>("ItemId")
                    .IsRequired();

                b.Property<string>("ModifiedBy")
                    .HasMaxLength(64);

                b.Property<DateTime?>("ModifiedDate");

                b.Property<int>("Priority");

                b.Property<int?>("Quantity");

                b.Property<string>("Tags")
                    .HasMaxLength(1024);

                b.HasKey("Id");

                b.HasIndex("AssociatedCategoryId");

                b.HasIndex("AssociatedItemId");

                b.HasIndex("ItemId");

                b.ToTable("Association");
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.CatalogEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<string>("CreatedBy")
                    .HasMaxLength(64);

                b.Property<DateTime>("CreatedDate");

                b.Property<string>("DefaultLanguage")
                    .IsRequired()
                    .HasMaxLength(64);

                b.Property<string>("ModifiedBy")
                    .HasMaxLength(64);

                b.Property<DateTime?>("ModifiedDate");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(128);

                b.Property<string>("OwnerId")
                    .HasMaxLength(128);

                b.Property<bool>("Virtual");

                b.HasKey("Id");

                b.ToTable("Catalog");
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.CatalogLanguageEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<string>("CatalogId")
                    .IsRequired();

                b.Property<string>("Language")
                    .HasMaxLength(64);

                b.HasKey("Id");

                b.HasIndex("CatalogId");

                b.ToTable("CatalogLanguage");
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.CategoryEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<string>("CatalogId")
                    .IsRequired()
                    .HasMaxLength(128);

                b.Property<string>("Code")
                    .IsRequired()
                    .HasMaxLength(64);

                b.Property<string>("CreatedBy")
                    .HasMaxLength(64);

                b.Property<DateTime>("CreatedDate");

                b.Property<DateTime?>("EndDate");

                b.Property<bool>("IsActive");

                b.Property<string>("ModifiedBy")
                    .HasMaxLength(64);

                b.Property<DateTime?>("ModifiedDate");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(128);

                b.Property<string>("ParentCategoryId")
                    .HasMaxLength(128);

                b.Property<int>("Priority");

                b.Property<DateTime>("StartDate");

                b.Property<string>("TaxType")
                    .HasMaxLength(64);

                b.HasKey("Id");

                b.HasIndex("CatalogId");

                b.HasIndex("ParentCategoryId");

                b.ToTable("Category");
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.CategoryItemRelationEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<string>("CatalogId")
                    .IsRequired();

                b.Property<string>("CategoryId");

                b.Property<string>("ItemId")
                    .IsRequired();

                b.Property<int>("Priority");

                b.HasKey("Id");

                b.HasIndex("CatalogId");

                b.HasIndex("CategoryId");

                b.HasIndex("ItemId");

                b.ToTable("CategoryItemRelation");
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.CategoryRelationEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<string>("SourceCategoryId")
                    .IsRequired();

                b.Property<string>("TargetCatalogId");

                b.Property<string>("TargetCategoryId");

                b.HasKey("Id");

                b.HasIndex("SourceCategoryId");

                b.HasIndex("TargetCatalogId");

                b.HasIndex("TargetCategoryId");

                b.ToTable("CategoryRelation");
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.EditorialReviewEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<string>("Comments");

                b.Property<string>("Content");

                b.Property<string>("CreatedBy")
                    .HasMaxLength(64);

                b.Property<DateTime>("CreatedDate");

                b.Property<string>("ItemId")
                    .IsRequired();

                b.Property<string>("Locale")
                    .HasMaxLength(64);

                b.Property<string>("ModifiedBy")
                    .HasMaxLength(64);

                b.Property<DateTime?>("ModifiedDate");

                b.Property<int>("Priority");

                b.Property<int>("ReviewState");

                b.Property<string>("Source")
                    .HasMaxLength(128);

                b.HasKey("Id");

                b.HasIndex("ItemId");

                b.ToTable("EditorialReview");
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.ImageEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<string>("CategoryId");

                b.Property<string>("CreatedBy")
                    .HasMaxLength(64);

                b.Property<DateTime>("CreatedDate");

                b.Property<string>("Group")
                    .HasMaxLength(64);

                b.Property<string>("ItemId");

                b.Property<string>("LanguageCode")
                    .HasMaxLength(5);

                b.Property<string>("ModifiedBy")
                    .HasMaxLength(64);

                b.Property<DateTime?>("ModifiedDate");

                b.Property<string>("Name")
                    .HasMaxLength(1024);

                b.Property<int>("SortOrder");

                b.Property<string>("Url")
                    .IsRequired()
                    .HasMaxLength(2083);

                b.HasKey("Id");

                b.HasIndex("CategoryId");

                b.HasIndex("ItemId");

                b.ToTable("CatalogImage");
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.ItemEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<int>("AvailabilityRule");

                b.Property<string>("CatalogId")
                    .IsRequired();

                b.Property<string>("CategoryId");

                b.Property<string>("Code")
                    .IsRequired()
                    .HasMaxLength(64);

                b.Property<string>("CreatedBy")
                    .HasMaxLength(64);

                b.Property<DateTime>("CreatedDate");

                b.Property<DateTime?>("DownloadExpiration");

                b.Property<string>("DownloadType")
                    .HasMaxLength(64);

                b.Property<bool?>("EnableReview");

                b.Property<DateTime?>("EndDate");

                b.Property<string>("Gtin")
                    .HasMaxLength(64);

                b.Property<bool?>("HasUserAgreement");

                b.Property<decimal?>("Height");

                b.Property<bool>("IsActive");

                b.Property<bool>("IsBuyable");

                b.Property<decimal?>("Length");

                b.Property<string>("ManufacturerPartNumber")
                    .HasMaxLength(128);

                b.Property<int?>("MaxNumberOfDownload");

                b.Property<decimal>("MaxQuantity");

                b.Property<string>("MeasureUnit")
                    .HasMaxLength(32);

                b.Property<decimal>("MinQuantity");

                b.Property<string>("ModifiedBy")
                    .HasMaxLength(64);

                b.Property<DateTime?>("ModifiedDate");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(1024);

                b.Property<string>("PackageType")
                    .HasMaxLength(128);

                b.Property<string>("ParentId");

                b.Property<int>("Priority");

                b.Property<string>("ProductType")
                    .HasMaxLength(64);

                b.Property<string>("ShippingType")
                    .HasMaxLength(64);

                b.Property<DateTime>("StartDate");

                b.Property<string>("TaxType")
                    .HasMaxLength(64);

                b.Property<bool>("TrackInventory");

                b.Property<string>("Vendor")
                    .HasMaxLength(128);

                b.Property<decimal?>("Weight");

                b.Property<string>("WeightUnit")
                    .HasMaxLength(32);

                b.Property<decimal?>("Width");

                b.HasKey("Id");

                b.HasIndex("CategoryId");

                b.HasIndex("Code")
                    .IsUnique();

                b.HasIndex("ParentId");

                b.HasIndex("CatalogId", "ParentId")
                    .HasName("IX_CatalogId_ParentId");

                b.ToTable("Item");
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.PropertyAttributeEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<string>("CreatedBy")
                    .HasMaxLength(64);

                b.Property<DateTime>("CreatedDate");

                b.Property<string>("ModifiedBy")
                    .HasMaxLength(64);

                b.Property<DateTime?>("ModifiedDate");

                b.Property<int>("Priority");

                b.Property<string>("PropertyAttributeName")
                    .IsRequired()
                    .HasMaxLength(128);

                b.Property<string>("PropertyAttributeValue")
                    .IsRequired()
                    .HasMaxLength(128);

                b.Property<string>("PropertyId")
                    .IsRequired();

                b.HasKey("Id");

                b.HasIndex("PropertyId");

                b.ToTable("PropertyAttribute");
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.PropertyDictionaryItemEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<string>("Alias")
                    .IsRequired()
                    .HasMaxLength(512);

                b.Property<string>("PropertyId")
                    .IsRequired();

                b.Property<int>("SortOrder");

                b.HasKey("Id");

                b.HasIndex("PropertyId");

                b.HasIndex("Alias", "PropertyId")
                    .IsUnique()
                    .HasName("IX_AliasAndPropertyId");

                b.ToTable("PropertyDictionaryItem");
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.PropertyDictionaryValueEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<string>("DictionaryItemId")
                    .IsRequired();

                b.Property<string>("Locale")
                    .HasMaxLength(64);

                b.Property<string>("Value")
                    .HasMaxLength(512);

                b.HasKey("Id");

                b.HasIndex("DictionaryItemId");

                b.ToTable("PropertyDictionaryValue");
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.PropertyDisplayNameEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<string>("Locale")
                    .HasMaxLength(64);

                b.Property<string>("Name")
                    .HasMaxLength(512);

                b.Property<string>("PropertyId")
                    .IsRequired();

                b.HasKey("Id");

                b.HasIndex("PropertyId");

                b.ToTable("PropertyDisplayName");
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.PropertyEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<bool>("AllowAlias");

                b.Property<string>("CatalogId");

                b.Property<string>("CategoryId");

                b.Property<string>("CreatedBy")
                    .HasMaxLength(64);

                b.Property<DateTime>("CreatedDate");

                b.Property<bool>("IsEnum");

                b.Property<bool>("IsHidden");

                b.Property<bool>("IsInput");

                b.Property<bool>("IsKey");

                b.Property<bool>("IsLocaleDependant");

                b.Property<bool>("IsMultiValue");

                b.Property<bool>("IsRequired");

                b.Property<bool>("IsSale");

                b.Property<string>("ModifiedBy")
                    .HasMaxLength(64);

                b.Property<DateTime?>("ModifiedDate");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(128);

                b.Property<int>("PropertyValueType");

                b.Property<string>("TargetType")
                    .HasMaxLength(128);

                b.HasKey("Id");

                b.HasIndex("CatalogId");

                b.HasIndex("CategoryId");

                b.ToTable("Property");
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.PropertyValidationRuleEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<int?>("CharCountMax");

                b.Property<int?>("CharCountMin");

                b.Property<bool>("IsUnique");

                b.Property<string>("PropertyId")
                    .IsRequired();

                b.Property<string>("RegExp")
                    .HasMaxLength(2048);

                b.HasKey("Id");

                b.HasIndex("PropertyId");

                b.ToTable("PropertyValidationRule");
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.PropertyValueEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<bool>("BooleanValue");

                b.Property<string>("CatalogId");

                b.Property<string>("CategoryId");

                b.Property<string>("CreatedBy")
                    .HasMaxLength(64);

                b.Property<DateTime>("CreatedDate");

                b.Property<DateTime?>("DateTimeValue");

                b.Property<decimal>("DecimalValue");

                b.Property<string>("DictionaryItemId");

                b.Property<int>("IntegerValue");

                b.Property<string>("ItemId");

                b.Property<string>("Locale")
                    .HasMaxLength(64);

                b.Property<string>("LongTextValue");

                b.Property<string>("ModifiedBy")
                    .HasMaxLength(64);

                b.Property<DateTime?>("ModifiedDate");

                b.Property<string>("Name")
                    .HasMaxLength(64);

                b.Property<string>("ShortTextValue")
                    .HasMaxLength(512);

                b.Property<int>("ValueType");

                b.HasKey("Id");

                b.HasIndex("CatalogId");

                b.HasIndex("CategoryId");

                b.HasIndex("DictionaryItemId");

                b.HasIndex("ItemId");

                b.ToTable("PropertyValue");
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.SeoInfoEntity", b =>
            {
                b.Property<string>("Id")
                    .ValueGeneratedOnAdd()
                    .HasMaxLength(128);

                b.Property<string>("CategoryId");

                b.Property<string>("CreatedBy")
                    .HasMaxLength(64);

                b.Property<DateTime>("CreatedDate");

                b.Property<string>("ImageAltDescription")
                    .HasMaxLength(255);

                b.Property<bool>("IsActive");

                b.Property<string>("ItemId");

                b.Property<string>("Keyword")
                    .IsRequired()
                    .HasMaxLength(255);

                b.Property<string>("Language")
                    .HasMaxLength(5);

                b.Property<string>("MetaDescription")
                    .HasMaxLength(1024);

                b.Property<string>("MetaKeywords")
                    .HasMaxLength(255);

                b.Property<string>("ModifiedBy")
                    .HasMaxLength(64);

                b.Property<DateTime?>("ModifiedDate");

                b.Property<string>("StoreId")
                    .HasMaxLength(128);

                b.Property<string>("Title")
                    .HasMaxLength(255);

                b.HasKey("Id");

                b.HasIndex("CategoryId");

                b.HasIndex("ItemId");

                b.ToTable("CatalogSeoInfo");
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.AssetEntity", b =>
            {
                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.ItemEntity", "CatalogItem")
                    .WithMany("Assets")
                    .HasForeignKey("ItemId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.AssociationEntity", b =>
            {
                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.CategoryEntity", "AssociatedCategory")
                    .WithMany()
                    .HasForeignKey("AssociatedCategoryId")
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.ItemEntity", "AssociatedItem")
                    .WithMany("ReferencedAssociations")
                    .HasForeignKey("AssociatedItemId")
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.ItemEntity", "Item")
                    .WithMany("Associations")
                    .HasForeignKey("ItemId")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.CatalogLanguageEntity", b =>
            {
                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.CatalogEntity", "Catalog")
                    .WithMany("CatalogLanguages")
                    .HasForeignKey("CatalogId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.CategoryEntity", b =>
            {
                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.CatalogEntity", "Catalog")
                    .WithMany()
                    .HasForeignKey("CatalogId")
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.CategoryEntity", "ParentCategory")
                    .WithMany()
                    .HasForeignKey("ParentCategoryId")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.CategoryItemRelationEntity", b =>
            {
                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.CatalogEntity", "Catalog")
                    .WithMany()
                    .HasForeignKey("CatalogId")
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.CategoryEntity", "Category")
                    .WithMany()
                    .HasForeignKey("CategoryId")
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.ItemEntity", "CatalogItem")
                    .WithMany("CategoryLinks")
                    .HasForeignKey("ItemId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.CategoryRelationEntity", b =>
            {
                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.CategoryEntity", "SourceCategory")
                    .WithMany("OutgoingLinks")
                    .HasForeignKey("SourceCategoryId")
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.CatalogEntity", "TargetCatalog")
                    .WithMany("IncommingLinks")
                    .HasForeignKey("TargetCatalogId")
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.CategoryEntity", "TargetCategory")
                    .WithMany("IncommingLinks")
                    .HasForeignKey("TargetCategoryId")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.EditorialReviewEntity", b =>
            {
                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.ItemEntity", "CatalogItem")
                    .WithMany("EditorialReviews")
                    .HasForeignKey("ItemId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.ImageEntity", b =>
            {
                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.CategoryEntity", "Category")
                    .WithMany("Images")
                    .HasForeignKey("CategoryId")
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.ItemEntity", "CatalogItem")
                    .WithMany("Images")
                    .HasForeignKey("ItemId")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.ItemEntity", b =>
            {
                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.CatalogEntity", "Catalog")
                    .WithMany()
                    .HasForeignKey("CatalogId")
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.CategoryEntity", "Category")
                    .WithMany()
                    .HasForeignKey("CategoryId")
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.ItemEntity", "Parent")
                    .WithMany("Childrens")
                    .HasForeignKey("ParentId")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.PropertyAttributeEntity", b =>
            {
                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.PropertyEntity", "Property")
                    .WithMany("PropertyAttributes")
                    .HasForeignKey("PropertyId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.PropertyDictionaryItemEntity", b =>
            {
                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.PropertyEntity", "Property")
                    .WithMany("DictionaryItems")
                    .HasForeignKey("PropertyId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.PropertyDictionaryValueEntity", b =>
            {
                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.PropertyDictionaryItemEntity", "DictionaryItem")
                    .WithMany("DictionaryItemValues")
                    .HasForeignKey("DictionaryItemId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.PropertyDisplayNameEntity", b =>
            {
                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.PropertyEntity", "Property")
                    .WithMany("DisplayNames")
                    .HasForeignKey("PropertyId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.PropertyEntity", b =>
            {
                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.CatalogEntity", "Catalog")
                    .WithMany("Properties")
                    .HasForeignKey("CatalogId")
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.CategoryEntity", "Category")
                    .WithMany("Properties")
                    .HasForeignKey("CategoryId")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.PropertyValidationRuleEntity", b =>
            {
                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.PropertyEntity", "Property")
                    .WithMany("ValidationRules")
                    .HasForeignKey("PropertyId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.PropertyValueEntity", b =>
            {
                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.CatalogEntity", "Catalog")
                    .WithMany("CatalogPropertyValues")
                    .HasForeignKey("CatalogId")
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.CategoryEntity", "Category")
                    .WithMany("CategoryPropertyValues")
                    .HasForeignKey("CategoryId")
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.PropertyDictionaryItemEntity", "DictionaryItem")
                    .WithMany()
                    .HasForeignKey("DictionaryItemId")
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.ItemEntity", "CatalogItem")
                    .WithMany("ItemPropertyValues")
                    .HasForeignKey("ItemId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("VirtoCommerce.CatalogModule.Data.Model.SeoInfoEntity", b =>
            {
                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.CategoryEntity", "Category")
                    .WithMany("SeoInfos")
                    .HasForeignKey("CategoryId")
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne("VirtoCommerce.CatalogModule.Data.Model.ItemEntity", "Item")
                    .WithMany("SeoInfos")
                    .HasForeignKey("ItemId")
                    .OnDelete(DeleteBehavior.Restrict);
            });
#pragma warning restore 612, 618
        }
    }
}
