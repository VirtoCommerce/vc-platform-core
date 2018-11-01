using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MockQueryable.Moq;
using Moq;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Core.Services;
using VirtoCommerce.ImageToolsModule.Data.Models;
using VirtoCommerce.ImageToolsModule.Data.Repositories;
using VirtoCommerce.ImageToolsModule.Data.Services;
using Xunit;

namespace VirtoCommerce.ImageToolsModule.Tests
{
    public class ThumbnailTaskSearchServiceTests
    {
        [Fact]
        public void Search_ThumbnailOptionSearchCriteria_ReturnsGenericSearchResponseOfTasksInExpectedOrder()
        {
            // Arrange
            var target = CreateTargetService();
            var criteria = new ThumbnailTaskSearchCriteria { Sort = "Name:desc;WorkPath:desc" };
            var expectedTasks = ThumbnailTaskEntitiesDataSource.Select(x => x.ToModel(new ThumbnailTask())).OrderByDescending(t => t.Name).ThenByDescending(t => t.WorkPath).ToArray();

            // Act
            var resultTasks = target.SearchAsync(criteria);

            // Assert
            Assert.Equal(expectedTasks, resultTasks.Result.Results);
        }

        [Fact]
        public void Search_SearchByExistingKeyword_TasksFound()
        {
            // Arrange
            var keyword = "NameLong";
            var target = CreateTargetService();
            var count = ThumbnailTaskEntitiesDataSource.Count(x => x.Name.Contains(keyword));

            // Act
            var resultTasks = target.SearchAsync(new ThumbnailTaskSearchCriteria { Keyword = keyword });

            // Assert
            Assert.Equal(resultTasks.Result.Results.Count, count);
        }

        private ThumbnailTaskSearchService CreateTargetService()
        {
            var entities = ThumbnailTaskEntitiesDataSource.ToList();
            var entitiesQueryableMock = entities.AsQueryable().BuildMock();
            var repoMock = new Mock<IThumbnailRepository>();

            repoMock.Setup(x => x.ThumbnailTasks).Returns(entitiesQueryableMock.Object);

            repoMock.Setup(x => x.GetThumbnailTasksByIdsAsync(It.IsAny<string[]>()))
                .ReturnsAsync((string[] ids) =>
                {
                    return entitiesQueryableMock.Object.Where(t => ids.Contains(t.Id)).ToArray();
                });

            var thumbnailTaskServiceMock = new Mock<IThumbnailTaskService>();
            thumbnailTaskServiceMock.Setup(x => x.GetByIdsAsync(It.IsAny<string[]>()))
                .ReturnsAsync((string[] ids) =>
                {
                    return ThumbnailTaskEntitiesDataSource.Where(entity => ids.Contains(entity.Id))
                        .Select(entity => entity.ToModel(new ThumbnailTask()))
                        .ToArray();
                });

            return new ThumbnailTaskSearchService(() => repoMock.Object, thumbnailTaskServiceMock.Object);
        }

        private IEnumerable<ThumbnailTaskEntity> ThumbnailTaskEntitiesDataSource
        {
            get
            {
                yield return new ThumbnailTaskEntity { Id = "Task1", Name = "Name 1", WorkPath = "Path 4" };
                yield return new ThumbnailTaskEntity { Id = "Task2", Name = "NameLong 2", WorkPath = "Path 3" };
                yield return new ThumbnailTaskEntity { Id = "Task3", Name = "Name 3", WorkPath = "Path 2" };
                yield return new ThumbnailTaskEntity { Id = "Task4", Name = "NameLong 4", WorkPath = "Path 1" };
            }
        }
    }
}
