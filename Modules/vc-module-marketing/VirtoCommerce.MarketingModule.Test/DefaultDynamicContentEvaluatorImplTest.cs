using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using Moq;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.CoreModule.Core.Conditions.Browse;
using VirtoCommerce.CoreModule.Core.Conditions.GeoConditions;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Model.DynamicContent;
using VirtoCommerce.MarketingModule.Core.Services;
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
        private readonly Mock<IDynamicContentService> _dynamicContentServiceMock;
        private readonly Mock<ILogger<DefaultDynamicContentEvaluatorImpl>> _loggerMock;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        public DefaultDynamicContentEvaluatorImplTest()
        {
            _repositoryMock = new Mock<IMarketingRepository>();

            _dynamicContentServiceMock = new Mock<IDynamicContentService>();
            //_expressionSerializerMock = new Mock<IExpressionSerializer>();
            _loggerMock = new Mock<ILogger<DefaultDynamicContentEvaluatorImpl>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _repositoryMock.Setup(ss => ss.UnitOfWork).Returns(_mockUnitOfWork.Object);

            AbstractTypeFactory<IConditionTree>.RegisterType<DynamicContentConditionTree>();
            AbstractTypeFactory<IConditionTree>.RegisterType<BlockContentCondition>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionGeoTimeZone>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionGeoZipCode>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionStoreSearchedPhrase>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionAgeIs>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionGeoCity>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionGeoCountry>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionGeoState>();
            AbstractTypeFactory<IConditionTree>.RegisterType<ConditionLanguageIs>();
            AbstractTypeFactory<IConditionTree>.RegisterType<UserGroupsContainsCondition>();
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
                    PredicateVisualTreeSerialized = GetConditionJson(),
                    IsActive = true,
                    ContentItems = new ObservableCollection<DynamicContentItem> { dynamicContentItem },
                    DynamicExpression = new DynamicContentConditionTree()
                }
            };
            _dynamicContentServiceMock.Setup(dcs => dcs.GetContentPublicationsByStoreIdAndPlaceNameAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .ReturnsAsync(groups.ToArray());
            _dynamicContentServiceMock.Setup(dcs => dcs.GetContentItemsByIdsAsync(new[] { dynamicContentItem.Id }))
                .ReturnsAsync(expectedArray);

            var evaluator = new DefaultDynamicContentEvaluatorImpl(_dynamicContentServiceMock.Object, _loggerMock.Object);

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
