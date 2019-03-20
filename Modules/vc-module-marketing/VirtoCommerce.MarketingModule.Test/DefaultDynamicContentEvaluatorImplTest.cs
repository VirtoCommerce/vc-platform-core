using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.MarketingModule.Data.Model;
using VirtoCommerce.MarketingModule.Data.Repositories;
using VirtoCommerce.MarketingModule.Data.Services;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Serialization;
using VirtoCommerce.Platform.Data.Serialization;
using Xunit;

namespace VirtoCommerce.MarketingModule.Test
{
    public class DefaultDynamicContentEvaluatorImplTest
    {
        private readonly Mock<IMarketingRepository> _repositoryMock;
        private Func<IMarketingRepository> _repositoryFactory;
        private readonly Mock<IDynamicContentService> _dynamicContentServiceMock;
        private readonly IExpressionSerializer _expressionSerializerMock;
        private readonly Mock<ILogger<DefaultDynamicContentEvaluatorImpl>> _loggerMock;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        public DefaultDynamicContentEvaluatorImplTest()
        {
            _repositoryMock = new Mock<IMarketingRepository>();

            _dynamicContentServiceMock = new Mock<IDynamicContentService>();
            //_expressionSerializerMock = new Mock<IExpressionSerializer>();
            _expressionSerializerMock = new XmlExpressionSerializer();
            _loggerMock = new Mock<ILogger<DefaultDynamicContentEvaluatorImpl>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _repositoryMock.Setup(ss => ss.UnitOfWork).Returns(_mockUnitOfWork.Object);
        }

        [Fact]
        public void EvaluateItemsAsync_Evaluate()
        {
            //TODO
            ////Arrange
            //var expected = new List<DynamicContentItem>();
            //var evalContext = new DynamicContentEvaluationContext();

            //var groups = new List<DynamicContentPublishingGroupEntity>
            //{
            //    new DynamicContentPublishingGroupEntity
            //    {
            //        ConditionExpression = GetConditionExpression(), IsActive = true,
            //        ContentPlaces = new ObservableCollection<PublishingGroupContentPlaceEntity>
            //        {
            //            new PublishingGroupContentPlaceEntity() {ContentPlace = new DynamicContentPlaceEntity()}
            //        }
            //    }
            //}.AsQueryable();
            //var evaluator = GetDefaultDynamicContentEvaluatorImpl(groups);

            ////Act
            //var items = evaluator.EvaluateItemsAsync(evalContext).GetAwaiter().GetResult();

            ////Assert
            //Assert.All(items, a => a.Description.Equals(string.Empty));
        }

        private DefaultDynamicContentEvaluatorImpl GetDefaultDynamicContentEvaluatorImpl(IQueryable<DynamicContentPublishingGroupEntity> groups)
        {
            var mockSet = MockDbSet.GetMockDbSet(groups);

            var options = new DbContextOptionsBuilder<MarketingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var mockContext = new Mock<MarketingDbContext>(options);
            mockContext.Setup(c => c.Set<DynamicContentPublishingGroupEntity>()).Returns(mockSet.Object);
            var mockDatabase = new Mock<DatabaseFacade>();
            mockContext.Setup(c => c.Database).Returns(mockDatabase.Object);
            var repository = new MarketingRepositoryImpl(mockContext.Object);
            _repositoryFactory = () => repository;
            return new DefaultDynamicContentEvaluatorImpl(_repositoryFactory, _dynamicContentServiceMock.Object, _loggerMock.Object);
        }

        private string GetConditionExpression()
        {
            return "<LambdaExpression NodeType=\"Lambda\" Name=\"\" TailCall=\"false\" CanReduce=\"false\"><Type><Type Name=\"System.Func`2\"><Type Name=\"VirtoCommerce.Domain.Common.IEvaluationContext\" /><Type Name=\"System.Boolean\" /></Type></Type><Parameters><ParameterExpression NodeType=\"Parameter\" Name=\"f\" IsByRef=\"false\" CanReduce=\"false\"><Type><Type Name=\"VirtoCommerce.Domain.Common.IEvaluationContext\" /></Type></ParameterExpression></Parameters><Body><ConstantExpression NodeType=\"Constant\" CanReduce=\"false\"><Type><Type Name=\"System.Boolean\" /></Type><Value>True</Value></ConstantExpression></Body><ReturnType><Type Name=\"System.Boolean\" /></ReturnType></LambdaExpression>";
        }
    }
}
