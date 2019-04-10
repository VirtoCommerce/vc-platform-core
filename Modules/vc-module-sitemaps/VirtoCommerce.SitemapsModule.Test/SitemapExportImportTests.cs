using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.SitemapsModule.Data.ExportImport;
using Xunit;

namespace VirtoCommerce.SitemapsModule.Test
{
    public class SitemapExportImportTests
    {
        private static readonly IList<Sitemap> TestSitemaps = new List<Sitemap>
        {
            new Sitemap
            {
                Location = "sitemap/vendor.xml",
                StoreId = "Electronics",
                UrlTemplate = "{slug}",
                TotalItemsCount = 1,
                CreatedDate = DateTime.Parse("2018-09-11T06:24:53.033"),
                ModifiedDate = DateTime.Parse("2018-09-11T06:24:53.033"),
                CreatedBy = "unknown",
                ModifiedBy = "unknown",
                Id = "007885500366414880da8d6e9ffda108"
            },
            new Sitemap
            {
                Location = "sitemap/products.xml",
                StoreId = "Clothing",
                UrlTemplate = "{language}/{slug}",
                TotalItemsCount = 3,
                CreatedDate = DateTime.Parse("2018-09-11T06:24:53.033"),
                ModifiedDate = DateTime.Parse("2018-09-11T06:24:53.033"),
                CreatedBy = "unknown",
                ModifiedBy = "unknown",
                Id = "0345316ed6b14fb290fe44c2420fff7b"
            },
            new Sitemap
            {
                Location = "sitemap/catalog.xml",
                StoreId = "Electronics",
                UrlTemplate = "{language}/{slug}",
                TotalItemsCount = 5,
                CreatedDate = DateTime.Parse("2018-09-11T06:24:53.033"),
                ModifiedDate = DateTime.Parse("2018-09-11T06:24:53.033"),
                CreatedBy = "unknown",
                ModifiedBy = "unknown",
                Id = "2c82debe589049a092543d41f675f233"
            },
            new Sitemap
            {
                Location = "sitemap/blog.xml",
                StoreId = "Electronics",
                UrlTemplate = "{language}/{slug}",
                TotalItemsCount = 3,
                CreatedDate = DateTime.Parse("2018-09-11T06:24:53.033"),
                ModifiedDate = DateTime.Parse("2018-09-11T06:24:53.033"),
                CreatedBy = "unknown",
                ModifiedBy = "unknown",
                Id = "91f6cee7b46c4b0a93a4068754b75a9f"
            }
        };

        private static readonly IList<SitemapItem> TestSitemapItems = new List<SitemapItem>
        {
            new SitemapItem
            {
                SitemapId = "0345316ed6b14fb290fe44c2420fff7b",
                Title = "Dresses",
                ImageUrl = "http://localhost/admin/assets/catalog/344394719.jpg",
                ObjectId = "7c51d90394f145d0859810e38a48a41e",
                ObjectType = "category",
                CreatedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                ModifiedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                CreatedBy = "unknown",
                ModifiedBy = "unknown",
                Id = "05c0e8a3f12f4a289a990ce45a69ed67"
            },
            new SitemapItem
            {
                SitemapId = "2c82debe589049a092543d41f675f233",
                Title = "Camcorders",
                ImageUrl = "http://localhost/admin/assets/catalog/1023206.jpg",
                ObjectId = "45d3fc9a913d4610a5c7d0470558c4b2",
                ObjectType = "category",
                CreatedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                ModifiedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                CreatedBy = "unknown",
                ModifiedBy = "unknown",
                Id = "0732a8fd70cb4bb5b7a81322671cfaa3"
            },
            new SitemapItem
            {
                SitemapId = "0345316ed6b14fb290fe44c2420fff7b",
                Title = "Shoes & Boots",
                ImageUrl = "http://localhost/admin/assets/catalog/MO049AWGCO07_1_v1.jpg",
                ObjectId = "ac56b04c5da54f038c53852f62810f27",
                ObjectType = "category",
                CreatedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                ModifiedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                CreatedBy = "unknown",
                ModifiedBy = "unknown",
                Id = "1088446f49624a9997f75b4e05857705"
            },
            new SitemapItem
            {
                SitemapId = "2c82debe589049a092543d41f675f233",
                Title = "Televisions",
                ImageUrl = "http://localhost/admin/assets/catalog/tv.jpg",
                ObjectId = "c76774f9047d4f18a916b38681c50557",
                ObjectType = "category",
                CreatedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                ModifiedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                CreatedBy = "unknown",
                ModifiedBy = "unknown",
                Id = "3e6844b107f7460bab431e140b0c71ff"
            },
            new SitemapItem
            {
                SitemapId = "2c82debe589049a092543d41f675f233",
                Title = "Headphones",
                ImageUrl = "http://localhost/admin/assets/catalog/headphones.jpg",
                ObjectId = "4b50b398ff584af9b6d69c082c94844b",
                ObjectType = "category",
                CreatedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                ModifiedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                CreatedBy = "unknown",
                ModifiedBy = "unknown",
                Id = "406a984aa3e24bef9c8d1ff5e1c9fca4"
            },
            new SitemapItem
            {
                SitemapId = "007885500366414880da8d6e9ffda108",
                Title = "samsung",
                ObjectId = "94597fe09f634914a5fdf76c6ba86b04",
                ObjectType = "Vendor",
                CreatedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                ModifiedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                CreatedBy = "unknown",
                ModifiedBy = "unknown",
                Id = "5485a9ff28aa4bdea4f1d9f2c63b452a"
            },
            new SitemapItem
            {
                SitemapId = "2c82debe589049a092543d41f675f233",
                Title = "Home Theater",
                ImageUrl = "http://localhost/admin/assets/catalog/home theater.jpg",
                ObjectId = "b1c093973bb24179bf130886b0477a18",
                ObjectType = "category",
                CreatedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                ModifiedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                CreatedBy = "unknown",
                ModifiedBy = "unknown",
                Id = "65b9d109fc0645bb9417116cf2edb366"
            },
            new SitemapItem
            {
                SitemapId = "91f6cee7b46c4b0a93a4068754b75a9f",
                Title = "blogs/news/awesome_post",
                ObjectType = "Custom",
                UrlTemplate = "blogs/news/awesome_post",
                CreatedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                ModifiedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                CreatedBy = "unknown",
                ModifiedBy = "unknown",
                Id = "6e0a24871800487bb1dc3b7c19b2373d"
            },
            new SitemapItem
            {
                SitemapId = "91f6cee7b46c4b0a93a4068754b75a9f",
                Title = "blogs/news",
                ObjectType = "Custom",
                UrlTemplate = "blogs/news",
                CreatedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                ModifiedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                CreatedBy = "unknown",
                ModifiedBy = "unknown",
                Id = "6f7ac82fe8814016b4c4e05738500966"
            },
            new SitemapItem
            {
                SitemapId = "0345316ed6b14fb290fe44c2420fff7b",
                Title = "Accessories",
                ImageUrl = "http://localhost/admin/assets/catalog/1_5b4cfc96-3fe2-4ee2-a554-a57febd1c666_large.jpeg",
                ObjectId = "8e4b6aa0df1a465799ab79bb54269c49",
                ObjectType = "category",
                CreatedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                ModifiedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                CreatedBy = "unknown",
                ModifiedBy = "unknown",
                Id = "a62dda7834a54296b67d91c44db21ce9"
            },
            new SitemapItem
            {
                SitemapId = "91f6cee7b46c4b0a93a4068754b75a9f",
                Title = "blogs/news/new_page2",
                ObjectType = "Custom",
                UrlTemplate = "blogs/news/new_page2",
                CreatedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                ModifiedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                CreatedBy = "unknown",
                ModifiedBy = "unknown",
                Id = "cc6a0201a37f46bb81a3d515fa72c995"
            },
            new SitemapItem
            {
                SitemapId = "2c82debe589049a092543d41f675f233",
                Title = "Cell phones",
                ImageUrl = "http://localhost/admin/assets/catalog/sell phones.jpg",
                ObjectId = "0d4ad9bab9184d69a6e586effdf9c2ea",
                ObjectType = "category",
                CreatedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                ModifiedDate = DateTime.Parse("2018-09-11T06:24:53.1"),
                CreatedBy = "unknown",
                ModifiedBy = "unknown",
                Id = "fdded563af9e4d8cb558b4e8bbdfff45"
            }
        };

        private readonly Mock<ISitemapService> _sitemapService;
        private readonly Mock<ISitemapItemService> _sitemapItemService;
        private readonly Mock<ICancellationToken> _cancellationToken;
        private readonly SitemapExportImport _sitemapExportImport;

        public SitemapExportImportTests()
        {
            _sitemapService = new Mock<ISitemapService>();
            _sitemapItemService = new Mock<ISitemapItemService>();
            _cancellationToken = new Mock<ICancellationToken>();

            InitSitemapService();
            InitSitemapItemService();

            var mvcJsonOptions = new MvcJsonOptions()
            {
                SerializerSettings =
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented
                }
            };

            var jsonOptions = new OptionsWrapper<MvcJsonOptions>(mvcJsonOptions);
            _sitemapExportImport = new SitemapExportImport(_sitemapService.Object, _sitemapItemService.Object, jsonOptions);
        }

        private static Stream ReadEmbeddedResource(string filePath)
        {
            var currentAssembly = typeof(SitemapExportImportTests).Assembly;
            var resourcePath = $"{currentAssembly.GetName().Name}.{filePath}";

            return currentAssembly.GetManifestResourceStream(resourcePath);
        }

        private static void IgnoreProgressInfo(ExportImportProgressInfo progressInfo)
        {
        }

        [Fact]
        public async Task TestDataExport()
        {

            string expectedJson;
            using (var resourceStream = ReadEmbeddedResource("Resources.SerializedSitemapsData.json"))
            using (var textReader = new StreamReader(resourceStream))
            {
                expectedJson = await textReader.ReadToEndAsync();
            }

            // Act
            string actualJson;
            using (var targetStream = new MemoryStream())
            {
                await _sitemapExportImport.DoExportAsync(targetStream, IgnoreProgressInfo, _cancellationToken.Object);

                var targetStreamContents = targetStream.ToArray();
                using (var copiedStream = new MemoryStream(targetStreamContents))
                using (var textReader = new StreamReader(copiedStream))
                {
                    actualJson = textReader.ReadToEnd();
                }
            }

            // Assert
            var expectedJObject = JsonConvert.DeserializeObject<JObject>(expectedJson);
            var actualJObject = JsonConvert.DeserializeObject<JObject>(actualJson);
            Assert.True(JToken.DeepEquals(expectedJObject, actualJObject));
        }

        [Fact]
        public async Task TestDataImport()
        {
            // Arrange
            Sitemap[] actualSitemaps = null;
            _sitemapService.Setup(service => service.SaveChangesAsync(It.IsAny<Sitemap[]>()))
                .Callback<Sitemap[]>(sitemaps => actualSitemaps = sitemaps)
                .Returns(Task.CompletedTask);

            SitemapItem[] actualSitemapItems = null;
            _sitemapItemService.Setup(service => service.SaveChangesAsync(It.IsAny<SitemapItem[]>()))
                .Callback<SitemapItem[]>(sitemapItems => actualSitemapItems = sitemapItems)
                .Returns(Task.CompletedTask);

            // Act
            using (var resourceStream = ReadEmbeddedResource("Resources.SerializedSitemapsData.json"))
            {
                await _sitemapExportImport.DoImportAsync(resourceStream, IgnoreProgressInfo, _cancellationToken.Object);
            }

            // Assert
            Assert.Equal(TestSitemaps, actualSitemaps);
            Assert.Equal(TestSitemapItems, actualSitemapItems);
        }

        private void InitSitemapService()
        {
            var sitemapSearchResult = new GenericSearchResult<Sitemap>
            {
                TotalCount = TestSitemaps.Count,
                Results = TestSitemaps
            };

            _sitemapService.Setup(service => service.SearchAsync(It.IsAny<SitemapSearchCriteria>()))
                .ReturnsAsync(sitemapSearchResult);
        }

        private void InitSitemapItemService()
        {
            var sitemapItemsSearchResult = new GenericSearchResult<SitemapItem>
            {
                TotalCount = TestSitemapItems.Count,
                Results = TestSitemapItems
            };

            _sitemapItemService.Setup(service => service.SearchAsync(It.IsAny<SitemapItemSearchCriteria>()))
                .ReturnsAsync(sitemapItemsSearchResult);
        }
    }
}
