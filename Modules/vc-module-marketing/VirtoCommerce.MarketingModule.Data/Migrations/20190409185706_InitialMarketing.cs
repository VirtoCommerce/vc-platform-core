using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.MarketingModule.Data.Migrations
{
    public partial class InitialMarketing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DynamicContentFolder",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    ImageUrl = table.Column<string>(maxLength: 2048, nullable: true),
                    ParentFolderId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicContentFolder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DynamicContentFolder_DynamicContentFolder_ParentFolderId",
                        column: x => x.ParentFolderId,
                        principalTable: "DynamicContentFolder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DynamicContentPublishingGroup",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    StoreId = table.Column<string>(maxLength: 256, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    ConditionExpression = table.Column<string>(nullable: true),
                    PredicateVisualTreeSerialized = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicContentPublishingGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Promotion",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    StoreId = table.Column<string>(maxLength: 128, nullable: true),
                    CatalogId = table.Column<string>(maxLength: 128, nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 1024, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    IsExclusive = table.Column<bool>(nullable: false),
                    IsAllowCombiningWithSelf = table.Column<bool>(nullable: false),
                    PredicateSerialized = table.Column<string>(nullable: true),
                    PredicateVisualTreeSerialized = table.Column<string>(nullable: true),
                    RewardsSerialized = table.Column<string>(nullable: true),
                    PerCustomerLimit = table.Column<int>(nullable: false),
                    TotalLimit = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DynamicContentItem",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    ContentTypeId = table.Column<string>(maxLength: 64, nullable: true),
                    IsMultilingual = table.Column<bool>(nullable: false),
                    ImageUrl = table.Column<string>(maxLength: 2048, nullable: true),
                    FolderId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicContentItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DynamicContentItem_DynamicContentFolder_FolderId",
                        column: x => x.FolderId,
                        principalTable: "DynamicContentFolder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DynamicContentPlace",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    ImageUrl = table.Column<string>(maxLength: 2048, nullable: true),
                    FolderId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicContentPlace", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DynamicContentPlace_DynamicContentFolder_FolderId",
                        column: x => x.FolderId,
                        principalTable: "DynamicContentFolder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Coupon",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Code = table.Column<string>(maxLength: 64, nullable: true),
                    MaxUsesNumber = table.Column<int>(nullable: false),
                    MaxUsesPerUser = table.Column<int>(nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: true),
                    PromotionId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coupon_Promotion_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromotionStore",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    PromotionId = table.Column<string>(nullable: false),
                    StoreId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionStore", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionStore_Promotion_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromotionUsage",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ObjectId = table.Column<string>(maxLength: 128, nullable: true),
                    ObjectType = table.Column<string>(maxLength: 128, nullable: true),
                    CouponCode = table.Column<string>(maxLength: 64, nullable: true),
                    UserId = table.Column<string>(maxLength: 128, nullable: true),
                    UserName = table.Column<string>(maxLength: 128, nullable: true),
                    PromotionId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionUsage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionUsage_Promotion_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PublishingGroupContentItem",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    DynamicContentPublishingGroupId = table.Column<string>(nullable: false),
                    DynamicContentItemId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishingGroupContentItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublishingGroupContentItem_DynamicContentItem_DynamicContentItemId",
                        column: x => x.DynamicContentItemId,
                        principalTable: "DynamicContentItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublishingGroupContentItem_DynamicContentPublishingGroup_DynamicContentPublishingGroupId",
                        column: x => x.DynamicContentPublishingGroupId,
                        principalTable: "DynamicContentPublishingGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PublishingGroupContentPlace",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    DynamicContentPublishingGroupId = table.Column<string>(nullable: false),
                    DynamicContentPlaceId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishingGroupContentPlace", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublishingGroupContentPlace_DynamicContentPlace_DynamicContentPlaceId",
                        column: x => x.DynamicContentPlaceId,
                        principalTable: "DynamicContentPlace",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublishingGroupContentPlace_DynamicContentPublishingGroup_DynamicContentPublishingGroupId",
                        column: x => x.DynamicContentPublishingGroupId,
                        principalTable: "DynamicContentPublishingGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_PromotionId",
                table: "Coupon",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_CodeAndPromotionId",
                table: "Coupon",
                columns: new[] { "Code", "PromotionId" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicContentFolder_ParentFolderId",
                table: "DynamicContentFolder",
                column: "ParentFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicContentItem_FolderId",
                table: "DynamicContentItem",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicContentPlace_FolderId",
                table: "DynamicContentPlace",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionStore_PromotionId",
                table: "PromotionStore",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionStore_StoreId",
                table: "PromotionStore",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionUsage_PromotionId",
                table: "PromotionUsage",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishingGroupContentItem_DynamicContentItemId",
                table: "PublishingGroupContentItem",
                column: "DynamicContentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishingGroupContentItem_DynamicContentPublishingGroupId",
                table: "PublishingGroupContentItem",
                column: "DynamicContentPublishingGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishingGroupContentPlace_DynamicContentPlaceId",
                table: "PublishingGroupContentPlace",
                column: "DynamicContentPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishingGroupContentPlace_DynamicContentPublishingGroupId",
                table: "PublishingGroupContentPlace",
                column: "DynamicContentPublishingGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupon");

            migrationBuilder.DropTable(
                name: "PromotionStore");

            migrationBuilder.DropTable(
                name: "PromotionUsage");

            migrationBuilder.DropTable(
                name: "PublishingGroupContentItem");

            migrationBuilder.DropTable(
                name: "PublishingGroupContentPlace");

            migrationBuilder.DropTable(
                name: "Promotion");

            migrationBuilder.DropTable(
                name: "DynamicContentItem");

            migrationBuilder.DropTable(
                name: "DynamicContentPlace");

            migrationBuilder.DropTable(
                name: "DynamicContentPublishingGroup");

            migrationBuilder.DropTable(
                name: "DynamicContentFolder");
        }
    }
}
