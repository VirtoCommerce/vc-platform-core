using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.CustomerModule.Data.Migrations
{
    public partial class InitialCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    MemberType = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    Discriminator = table.Column<string>(maxLength: 128, nullable: false),
                    FirstName = table.Column<string>(maxLength: 128, nullable: true),
                    MiddleName = table.Column<string>(maxLength: 128, nullable: true),
                    LastName = table.Column<string>(maxLength: 128, nullable: true),
                    FullName = table.Column<string>(maxLength: 254, nullable: true),
                    TimeZone = table.Column<string>(maxLength: 32, nullable: true),
                    DefaultLanguage = table.Column<string>(maxLength: 32, nullable: true),
                    BirthDate = table.Column<DateTime>(nullable: true),
                    TaxpayerId = table.Column<string>(maxLength: 64, nullable: true),
                    PreferredDelivery = table.Column<string>(maxLength: 64, nullable: true),
                    PreferredCommunication = table.Column<string>(maxLength: 64, nullable: true),
                    PhotoUrl = table.Column<string>(maxLength: 2083, nullable: true),
                    Salutation = table.Column<string>(maxLength: 256, nullable: true),
                    Type = table.Column<string>(maxLength: 64, nullable: true),
                    IsActive = table.Column<bool>(nullable: true),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    BusinessCategory = table.Column<string>(maxLength: 64, nullable: true),
                    OwnerId = table.Column<string>(maxLength: 128, nullable: true),
                    SiteUrl = table.Column<string>(maxLength: 2048, nullable: true),
                    LogoUrl = table.Column<string>(maxLength: 2048, nullable: true),
                    GroupName = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 2048, nullable: true),
                    FirstName = table.Column<string>(maxLength: 128, nullable: true),
                    LastName = table.Column<string>(maxLength: 128, nullable: true),
                    Line1 = table.Column<string>(maxLength: 128, nullable: false),
                    Line2 = table.Column<string>(maxLength: 128, nullable: true),
                    City = table.Column<string>(maxLength: 128, nullable: false),
                    CountryCode = table.Column<string>(maxLength: 64, nullable: false),
                    StateProvince = table.Column<string>(maxLength: 128, nullable: true),
                    CountryName = table.Column<string>(maxLength: 128, nullable: false),
                    PostalCode = table.Column<string>(maxLength: 32, nullable: false),
                    RegionId = table.Column<string>(maxLength: 128, nullable: true),
                    RegionName = table.Column<string>(maxLength: 128, nullable: true),
                    Type = table.Column<string>(maxLength: 64, nullable: true),
                    DaytimePhoneNumber = table.Column<string>(maxLength: 64, nullable: true),
                    EveningPhoneNumber = table.Column<string>(maxLength: 64, nullable: true),
                    FaxNumber = table.Column<string>(maxLength: 64, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    Organization = table.Column<string>(maxLength: 128, nullable: true),
                    MemberId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Address_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Email",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Address = table.Column<string>(maxLength: 254, nullable: true),
                    IsValidated = table.Column<bool>(nullable: false),
                    Type = table.Column<string>(maxLength: 64, nullable: true),
                    MemberId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Email", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Email_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberGroup",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Group = table.Column<string>(maxLength: 64, nullable: true),
                    MemberId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberGroup_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberRelation",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    AncestorSequence = table.Column<int>(nullable: false),
                    AncestorId = table.Column<string>(maxLength: 128, nullable: false),
                    RelationType = table.Column<string>(maxLength: 64, nullable: true),
                    DescendantId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberRelation_Member_AncestorId",
                        column: x => x.AncestorId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberRelation_Member_DescendantId",
                        column: x => x.DescendantId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateTable(
                name: "Note",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 64, nullable: true),
                    AuthorName = table.Column<string>(maxLength: 128, nullable: true),
                    ModifierName = table.Column<string>(maxLength: 128, nullable: true),
                    Title = table.Column<string>(maxLength: 128, nullable: true),
                    Body = table.Column<string>(nullable: true),
                    IsSticky = table.Column<bool>(nullable: false),
                    MemberId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Note", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Note_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Phone",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Number = table.Column<string>(maxLength: 64, nullable: true),
                    Type = table.Column<string>(maxLength: 64, nullable: true),
                    MemberId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Phone_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_MemberId",
                table: "Address",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Email_Address",
                table: "Email",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_Email_MemberId",
                table: "Email",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberType",
                table: "Member",
                column: "MemberType");

            migrationBuilder.CreateIndex(
                name: "IX_Member_Name",
                table: "Member",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MemberGroup_Group",
                table: "MemberGroup",
                column: "Group");

            migrationBuilder.CreateIndex(
                name: "IX_MemberGroup_MemberId",
                table: "MemberGroup",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberRelation_AncestorId",
                table: "MemberRelation",
                column: "AncestorId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberRelation_DescendantId",
                table: "MemberRelation",
                column: "DescendantId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberSeoInfo_MemberId",
                table: "MemberSeoInfo",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Note_MemberId",
                table: "Note",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Phone_MemberId",
                table: "Phone",
                column: "MemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Email");

            migrationBuilder.DropTable(
                name: "MemberGroup");

            migrationBuilder.DropTable(
                name: "MemberRelation");

            migrationBuilder.DropTable(
                name: "MemberSeoInfo");

            migrationBuilder.DropTable(
                name: "Note");

            migrationBuilder.DropTable(
                name: "Phone");

            migrationBuilder.DropTable(
                name: "Member");
        }
    }
}
