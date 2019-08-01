using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class ThumbnailOptionSearchServiceTests
    {
        [Fact]
        public void Search_ThumbnailOptionSearchCriteria_ReturnsGenericSearchResponseOfTasksInExpectedOrder()
        {
            var repoMock = GetOptionsRepositoryMock();
            var optionsServiceMock = new Moq.Mock<IThumbnailOptionService>();
           
            var target = new ThumbnailOptionSearchService(() => repoMock.Object, optionsServiceMock.Object);
            var criteria = new ThumbnailOptionSearchCriteria { Sort = "Name:desc;FileSuffix:desc" };
            var resultTasks = target.SearchAsync(criteria).GetAwaiter().GetResult();

            var expectedTasks = ThumbnailTaskEntitysDataSource.Select(x => x.ToModel(new ThumbnailOption())).OrderByDescending(t => t.Name).ThenByDescending(t => t.FileSuffix).ToArray();
            Assert.Equal(expectedTasks, resultTasks.Results);
        }

        public Mock<IThumbnailRepository> GetOptionsRepositoryMock()
        {
            var entities = ThumbnailTaskEntitysDataSource.ToList();
            var entitiesQueryableMock = entities.AsQueryable().BuildMock();
            var repoMock = new Mock<IThumbnailRepository>();

            repoMock.Setup(x => x.ThumbnailOptions).Returns(entitiesQueryableMock.Object);

            repoMock.Setup(x => x.GetThumbnailOptionsByIdsAsync(It.IsAny<string[]>()))
                .Returns((string[] ids) =>
                {
                    return Task.FromResult(entitiesQueryableMock.Object.Where(t => ids.Contains(t.Id)).ToArray());
                });

            return repoMock;
        }

        public IEnumerable<ThumbnailOptionEntity> ThumbnailTaskEntitysDataSource
        {
            get
            {
                yield return new ThumbnailOptionEntity { Id = "Option1", Name = "Name 1", FileSuffix = "SuffixName4" };
                yield return new ThumbnailOptionEntity { Id = "Option2", Name = "NameLong 2", FileSuffix = "SuffixName3" };
                yield return new ThumbnailOptionEntity { Id = "Option3", Name = "Name 3", FileSuffix = "SuffixName2" };
                yield return new ThumbnailOptionEntity { Id = "Option4", Name = "NameLong 4", FileSuffix = "SuffixName1" };
            }
        }
    }
}
