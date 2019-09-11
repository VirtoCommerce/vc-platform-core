using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.CoreModule.Core.Conditions.Browse;
using VirtoCommerce.CoreModule.Core.Conditions.GeoConditions;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Model.DynamicContent;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Search;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.MarketingModule.Data.Promotions;
using VirtoCommerce.MarketingModule.Data.Repositories;
using VirtoCommerce.MarketingModule.Data.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using Xunit;

namespace VirtoCommerce.MarketingModule.Test
{
    public class DefaultDynamicContentEvaluatorImplTest
    {
        private readonly Mock<IMarketingRepository> _repositoryMock;
        private readonly Mock<IContentPublicationsSearchService> _dynamicContentSearchServiceMock;
        private readonly Mock<IDynamicContentService> _dynamicContentServiceMock;
        private readonly Mock<ILogger<DefaultDynamicContentEvaluator>> _loggerMock;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        public DefaultDynamicContentEvaluatorImplTest()
        {
            _repositoryMock = new Mock<IMarketingRepository>();

            _dynamicContentSearchServiceMock = new Mock<IContentPublicationsSearchService>();
            _dynamicContentServiceMock = new Mock<IDynamicContentService>();
            _loggerMock = new Mock<ILogger<DefaultDynamicContentEvaluator>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _repositoryMock.Setup(ss => ss.UnitOfWork).Returns(_mockUnitOfWork.Object);

            AbstractTypeFactory<IConditionTree>.RegisterType<DynamicContentConditionTree>();
            foreach (var conditionTree in ((IConditionTree)AbstractTypeFactory<DynamicContentConditionTree>.TryCreateInstance()).Traverse(x => x.AvailableChildren))
            {
                AbstractTypeFactory<IConditionTree>.RegisterType(conditionTree.GetType());
            }
        }

        [Fact]
        public void EvaluateItemsAsync_Evaluate()
        {
            //Arrange
            var expected = new List<DynamicContentItem>();
            var evalContext = new DynamicContentEvaluationContext() { GeoCity = "NY" };
            var dynamicContentItem = new DynamicContentItem()
            {
                Id = Guid.NewGuid().ToString()
            };
            expected.Add(dynamicContentItem);
            var expectedArray = expected.ToArray();

            var groups = new List<DynamicContentPublication>
            {
                new DynamicContentPublication
                {
                    DynamicExpression = JsonConvert.DeserializeObject<DynamicContentConditionTree>(GetConditionJson(), new ConditionJsonConverter(), new RewardJsonConverter()),
                    IsActive = true,
                    ContentItems = new ObservableCollection<DynamicContentItem> { dynamicContentItem },
                }
            };
            _dynamicContentSearchServiceMock.Setup(dcs => dcs.SearchContentPublicationsAsync(It.IsAny<DynamicContentPublicationSearchCriteria>()))
                .ReturnsAsync(new Core.Model.DynamicContent.Search.DynamicContentPublicationSearchResult { Results = groups.ToArray() });
            _dynamicContentServiceMock.Setup(dcs => dcs.GetContentItemsByIdsAsync(new[] { dynamicContentItem.Id }))
                .ReturnsAsync(expectedArray);

            var evaluator = new DefaultDynamicContentEvaluator(_dynamicContentSearchServiceMock.Object, _dynamicContentServiceMock.Object, _loggerMock.Object);

            //Act
            var results = evaluator.EvaluateItemsAsync(evalContext).GetAwaiter().GetResult();

            //Assert
            Assert.Equal(expectedArray, results);
        }

        private string GetConditionJson()
        {
            return "{\"AvailableChildren\":null,\"Children\":[{\"All\":false,\"Not\":false,\"AvailableChildren\":null,\"Children\":[{\"Value\":\"NY\",\"MatchCondition\":\"Contains\",\"AvailableChildren\":null,\"Children\":[],\"Id\":\"ConditionGeoCity\"}],\"Id\":\"BlockContentCondition\"}],\"Id\":\"DynamicContentConditionTree\"}";
        }
    }
}
