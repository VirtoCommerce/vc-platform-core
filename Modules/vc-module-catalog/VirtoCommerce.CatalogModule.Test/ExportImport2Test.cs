using System.Linq;
using Moq;
using Newtonsoft.Json;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.ExportImport;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.ExportModule.Test.MockHelpers;
using Xunit;

namespace VirtoCommerce.CatalogModule.Test
{
    public class ExportImport2Test
    {
        [Fact]
        public void CatalogFetchAll()
        {
            // Arrange
            var catalogExportPagedDataSource = InitCatalogExportPagedDataSource();
            // Act
            var fetchResult = catalogExportPagedDataSource.FetchNextPage();
            // Assert
            Assert.Equal(2, fetchResult.Count());
        }

        [Fact]
        public void CatalogFetchForSpecificIds()
        {
            // Arrange
            var catalogExportPagedDataSource = InitCatalogExportPagedDataSource();
            catalogExportPagedDataSource.DataQuery.ObjectIds = new[] { "7829d35f417e4dd98851f51322f32c23" };
            // Act
            var fetchResult = catalogExportPagedDataSource.FetchNextPage();
            // Assert
            Assert.Single(fetchResult);
        }

        private CatalogExportPagedDataSource InitCatalogExportPagedDataSource()
        {
            var authorizationServicesMock = AuthMockHelper.AuthServicesMock(true);
            var сatalogSearchServiceMock = new Mock<ICatalogSearchService>();
            сatalogSearchServiceMock.Setup(x => x.SearchCatalogsAsync(It.IsAny<CatalogSearchCriteria>())).ReturnsAsync(GetTestCatalogSearchResult());
            var сatalogServiceMock = new Mock<ICatalogService>();
            сatalogServiceMock.Setup(x => x.GetByIdsAsync(new string[] { "7829d35f417e4dd98851f51322f32c23" }, null)).ReturnsAsync(GetTestCatalogByIdsResult());
            return new CatalogExportPagedDataSource(
                сatalogSearchServiceMock.Object,
                сatalogServiceMock.Object,
                authorizationServicesMock.AuthorizationPolicyProvider,
                authorizationServicesMock.AuthorizationService,
                authorizationServicesMock.UserClaimsPrincipalFactory,
                authorizationServicesMock.UserManager)
            {
                DataQuery = new CatalogExportDataQuery()
            };
        }

        private CatalogSearchResult GetTestCatalogSearchResult()
        {
            #region resultInJSON
            var resultInJSON = @"
{
  ""TotalCount"": 2,
  ""Results"": [
    {
      ""Name"": ""B2B-mixed"",
      ""IsVirtual"": true,
      ""OuterId"": null,
      ""DefaultLanguage"": {
        ""CatalogId"": null,
        ""IsDefault"": true,
        ""LanguageCode"": ""en-US"",
        ""Id"": null
      },
      ""Languages"": [
        {
          ""CatalogId"": null,
          ""IsDefault"": true,
          ""LanguageCode"": ""en-US"",
          ""Id"": null
        }
      ],
      ""CreatedDate"": ""0001-01-01T00:00:00"",
      ""ModifiedDate"": null,
      ""CreatedBy"": null,
      ""ModifiedBy"": null,
      ""Id"": ""fc596540864a41bf8ab78734ee7353a3""
    },
    {
      ""Name"": ""Bolts"",
      ""IsVirtual"": false,
      ""OuterId"": null,
      ""DefaultLanguage"": {
        ""CatalogId"": null,
        ""IsDefault"": true,
        ""LanguageCode"": ""en-US"",
        ""Id"": null
      },
      ""Languages"": [
        {
          ""CatalogId"": null,
          ""IsDefault"": true,
          ""LanguageCode"": ""en-US"",
          ""Id"": null
        }
      ],
      ""CreatedDate"": ""0001-01-01T00:00:00"",
      ""ModifiedDate"": null,
      ""CreatedBy"": null,
      ""ModifiedBy"": null,
      ""Id"": ""7829d35f417e4dd98851f51322f32c23""
    }
  ]
}
";
            #endregion
            return (CatalogSearchResult)JsonConvert.DeserializeObject(resultInJSON, typeof(CatalogSearchResult), new ConditionJsonConverter());
        }

        private Catalog[] GetTestCatalogByIdsResult()
        {
            #region resultInJSON
            var resultInJSON = @"
[
  {
    ""Name"": ""Bolts"",
    ""IsVirtual"": false,
    ""OuterId"": null,
    ""DefaultLanguage"": {
      ""CatalogId"": null,
      ""IsDefault"": true,
      ""LanguageCode"": ""en-US"",
      ""Id"": null
    },
    ""Languages"": [
      {
        ""CatalogId"": null,
        ""IsDefault"": true,
        ""LanguageCode"": ""en-US"",
        ""Id"": null
      }
    ],
    ""Properties"": [
      {
        ""IsReadOnly"": true,
        ""IsManageable"": true,
        ""IsNew"": false,
        ""CatalogId"": ""7829d35f417e4dd98851f51322f32c23"",
        ""CategoryId"": null,
        ""Name"": ""BRAND"",
        ""Required"": false,
        ""Dictionary"": true,
        ""Multivalue"": false,
        ""Multilanguage"": false,
        ""Hidden"": false,
        ""ValueType"": 0,
        ""Type"": 0,
        ""OuterId"": null,
        ""Values"": [],
        ""Attributes"": [],
        ""DisplayNames"": [],
        ""ValidationRules"": [],
        ""ValidationRule"": null,
        ""IsInherited"": false,
        ""Id"": ""75d8f2e5-ced1-4d65-b379-305793eb5780""
      },
      {
        ""IsReadOnly"": true,
        ""IsManageable"": true,
        ""IsNew"": false,
        ""CatalogId"": ""7829d35f417e4dd98851f51322f32c23"",
        ""CategoryId"": null,
        ""Name"": ""SYSTEM OF MEASUREMENT"",
        ""Required"": false,
        ""Dictionary"": true,
        ""Multivalue"": false,
        ""Multilanguage"": false,
        ""Hidden"": false,
        ""ValueType"": 0,
        ""Type"": 0,
        ""OuterId"": null,
        ""Values"": [],
        ""Attributes"": [],
        ""DisplayNames"": [],
        ""ValidationRules"": [],
        ""ValidationRule"": null,
        ""IsInherited"": false,
        ""Id"": ""7f46e4d0-9d4e-49e5-994a-d51ff0fbd239""
      }
    ],
    ""CreatedDate"": ""0001-01-01T00:00:00"",
    ""ModifiedDate"": null,
    ""CreatedBy"": null,
    ""ModifiedBy"": null,
    ""Id"": ""7829d35f417e4dd98851f51322f32c23""
  }
]
";
            #endregion
            return (Catalog[])JsonConvert.DeserializeObject(resultInJSON, typeof(Catalog[]));
        }
    }
}
