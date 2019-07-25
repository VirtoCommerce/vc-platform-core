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
            var catalogExportPagedDataSource = InitCatalogExportPagedDataSource();
            var fetchResult = catalogExportPagedDataSource.FetchNextPage();
            Assert.Equal(7, fetchResult.Count());
        }

        [Fact]
        public void CatalogFetchForSpecificIds()
        {
            var catalogExportPagedDataSource = InitCatalogExportPagedDataSource();
            catalogExportPagedDataSource.DataQuery.ObjectIds = new[] { "7829d35f417e4dd98851f51322f32c23" };
            var fetchResult = catalogExportPagedDataSource.FetchNextPage();
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
  ""TotalCount"": 7,
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
      ""Properties"": [],
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
    },
    {
      ""Name"": ""Clothing"",
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
          ""CatalogId"": ""b61aa9d1d0024bc4be12d79bf5786e9f"",
          ""CategoryId"": null,
          ""Name"": ""Brand"",
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
          ""Id"": ""4ab75b89-221d-49e2-a741-42c239b18df9""
        },
        {
          ""IsReadOnly"": true,
          ""IsManageable"": true,
          ""IsNew"": false,
          ""CatalogId"": ""b61aa9d1d0024bc4be12d79bf5786e9f"",
          ""CategoryId"": null,
          ""Name"": ""Colour"",
          ""Required"": false,
          ""Dictionary"": true,
          ""Multivalue"": false,
          ""Multilanguage"": false,
          ""Hidden"": false,
          ""ValueType"": 0,
          ""Type"": 1,
          ""OuterId"": null,
          ""Values"": [],
          ""Attributes"": [
            {
              ""PropertyId"": ""daa0fafb-4447-4af0-9ec1-920162f5bfbb"",
              ""Value"": ""Colour"",
              ""Name"": ""DisplayNameen-US"",
              ""CreatedDate"": ""2015-10-06T22:39:19.787"",
              ""ModifiedDate"": ""2015-10-06T22:39:19.787"",
              ""CreatedBy"": ""unknown"",
              ""ModifiedBy"": ""unknown"",
              ""Id"": ""a3b2985de60d493ab455328789b5b9a0""
            }
          ],
          ""DisplayNames"": [],
          ""ValidationRules"": [],
          ""ValidationRule"": null,
          ""IsInherited"": false,
          ""Id"": ""daa0fafb-4447-4af0-9ec1-920162f5bfbb""
        },
        {
          ""IsReadOnly"": true,
          ""IsManageable"": true,
          ""IsNew"": false,
          ""CatalogId"": ""b61aa9d1d0024bc4be12d79bf5786e9f"",
          ""CategoryId"": null,
          ""Name"": ""Size"",
          ""Required"": false,
          ""Dictionary"": false,
          ""Multivalue"": false,
          ""Multilanguage"": false,
          ""Hidden"": false,
          ""ValueType"": 0,
          ""Type"": 1,
          ""OuterId"": null,
          ""Values"": [],
          ""Attributes"": [
            {
              ""PropertyId"": ""717d2f8d-da59-479c-9db5-10f6d6249cc5"",
              ""Value"": ""Size"",
              ""Name"": ""DisplayNameen-US"",
              ""CreatedDate"": ""2015-10-06T22:39:19.907"",
              ""ModifiedDate"": ""2015-10-06T22:39:19.907"",
              ""CreatedBy"": ""unknown"",
              ""ModifiedBy"": ""unknown"",
              ""Id"": ""02627aa1736741239a0b1bfb0422ab0a""
            }
          ],
          ""DisplayNames"": [],
          ""ValidationRules"": [],
          ""ValidationRule"": null,
          ""IsInherited"": false,
          ""Id"": ""717d2f8d-da59-479c-9db5-10f6d6249cc5""
        },
        {
          ""IsReadOnly"": true,
          ""IsManageable"": true,
          ""IsNew"": false,
          ""CatalogId"": ""b61aa9d1d0024bc4be12d79bf5786e9f"",
          ""CategoryId"": null,
          ""Name"": ""Style"",
          ""Required"": false,
          ""Dictionary"": true,
          ""Multivalue"": true,
          ""Multilanguage"": false,
          ""Hidden"": false,
          ""ValueType"": 0,
          ""Type"": 0,
          ""OuterId"": null,
          ""Values"": [],
          ""Attributes"": [
            {
              ""PropertyId"": ""ed41aaee-e695-470f-88d2-d8c0dcf31e2d"",
              ""Value"": ""Style"",
              ""Name"": ""DisplayNameen-US"",
              ""CreatedDate"": ""2015-10-06T22:39:19.857"",
              ""ModifiedDate"": ""2015-10-06T22:39:19.857"",
              ""CreatedBy"": ""unknown"",
              ""ModifiedBy"": ""unknown"",
              ""Id"": ""fe4e5329bf9342738be7815e49de8043""
            }
          ],
          ""DisplayNames"": [],
          ""ValidationRules"": [],
          ""ValidationRule"": null,
          ""IsInherited"": false,
          ""Id"": ""ed41aaee-e695-470f-88d2-d8c0dcf31e2d""
        }
      ],
      ""CreatedDate"": ""0001-01-01T00:00:00"",
      ""ModifiedDate"": null,
      ""CreatedBy"": null,
      ""ModifiedBy"": null,
      ""Id"": ""b61aa9d1d0024bc4be12d79bf5786e9f""
    },
    {
      ""Name"": ""Clothing virtual"",
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
      ""Properties"": [],
      ""CreatedDate"": ""0001-01-01T00:00:00"",
      ""ModifiedDate"": null,
      ""CreatedBy"": null,
      ""ModifiedBy"": null,
      ""Id"": ""25f5ea1b52e54ec1aa903d44cc889324""
    },
    {
      ""Name"": ""Electronics"",
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
        },
        {
          ""CatalogId"": ""ecb5f43cdd934054a9eafb7da0160a71"",
          ""IsDefault"": false,
          ""LanguageCode"": ""de-DE"",
          ""Id"": ""ecb5f43cdd934054a9eafb7da0160a71""
        }
      ],
      ""Properties"": [
        {
          ""IsReadOnly"": true,
          ""IsManageable"": true,
          ""IsNew"": false,
          ""CatalogId"": ""4974648a41df4e6ea67ef2ad76d7bbd4"",
          ""CategoryId"": null,
          ""Name"": ""Brand"",
          ""Required"": true,
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
          ""Id"": ""43d14478-d142-4a65-956f-0a308d0c4ee8""
        }
      ],
      ""CreatedDate"": ""0001-01-01T00:00:00"",
      ""ModifiedDate"": null,
      ""CreatedBy"": null,
      ""ModifiedBy"": null,
      ""Id"": ""4974648a41df4e6ea67ef2ad76d7bbd4""
    },
    {
      ""Name"": ""Electronics virtual"",
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
      ""Properties"": [],
      ""CreatedDate"": ""0001-01-01T00:00:00"",
      ""ModifiedDate"": null,
      ""CreatedBy"": null,
      ""ModifiedBy"": null,
      ""Id"": ""1662f5e03af04275aa5d2653357d20e4""
    },
    {
      ""Name"": ""MFD"",
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
          ""CatalogId"": ""5aa50aaea01544529a6b6d576a668439"",
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
          ""Id"": ""18e4e3c0-b16e-4d28-b800-26a10c274caa""
        },
        {
          ""IsReadOnly"": true,
          ""IsManageable"": true,
          ""IsNew"": false,
          ""CatalogId"": ""5aa50aaea01544529a6b6d576a668439"",
          ""CategoryId"": null,
          ""Name"": ""OPERATING SYSTEM"",
          ""Required"": false,
          ""Dictionary"": true,
          ""Multivalue"": true,
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
          ""Id"": ""47a5eabb-4b88-41d1-81d7-8daf2b8bc157""
        }
      ],
      ""CreatedDate"": ""0001-01-01T00:00:00"",
      ""ModifiedDate"": null,
      ""CreatedBy"": null,
      ""ModifiedBy"": null,
      ""Id"": ""5aa50aaea01544529a6b6d576a668439""
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
