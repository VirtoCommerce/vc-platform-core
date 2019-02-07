using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.ImageToolsModule.Data.Models;
using VirtoCommerce.ImageToolsModule.Data.Repositories;
using VirtoCommerce.ImageToolsModule.Data.Services;
using VirtoCommerce.Platform.Core.Domain;
using Xunit;

namespace VirtoCommerce.ImageToolsModule.Tests
{
    public class ThumbnailOptionServiceTests
    {
        [Fact]
        public void GetByIds_ArrayOfIdis_ReturnsArrayOfThumbnailOption()
        {
            var optionEntites = ThumbnailOptionEntitesDataSource.ToArray();

            var ids = optionEntites.Select(t => t.Id).ToArray();
            var tasks = optionEntites.Select(t => t.ToModel(new ThumbnailOption())).ToArray();

            var mock = new Mock<IThumbnailRepository>();
            mock.Setup(r => r.GetThumbnailOptionsByIdsAsync(It.IsIn<string[]>(ids)))
                .ReturnsAsync(optionEntites.Where(o => ids.Contains(o.Id)).ToArray());

            var sut = new ThumbnailOptionService(() => mock.Object);
            var result = sut.GetByIdsAsync(ids);

            Assert.Equal(result.Result.Length, tasks.Length);
        }

        [Fact]
        public async Task Delete_ThumbnailOptionIds_DeletedThumbnailOptionWithPassedIds()
        {
            var optionEntites = ThumbnailOptionEntitesDataSource.ToList();

            var ids = optionEntites.Select(t => t.Id).ToArray();

            var mock = new Mock<IThumbnailRepository>();
            mock.SetupGet(x => x.UnitOfWork).Returns(new Mock<IUnitOfWork>().Object);
            mock.Setup(r => r.RemoveThumbnailOptionsByIds(It.IsIn<string[]>(ids)))
                .Callback((string[] arr) =>
                {
                    var entities = optionEntites.Where(e => arr.Contains(e.Id)).ToList();
                    foreach (var entity in entities)
                    {
                        optionEntites.Remove(entity);
                    }
                })
                .Returns(Task.CompletedTask);

            var sut = new ThumbnailOptionService(() => mock.Object);
            await sut.RemoveByIdsAsync(ids);

            Assert.Empty(optionEntites);
        }

        [Fact]
        public async Task SaveChanges_ArrayOfThumbnailOptions_ThumbnailOptionsUpdatedAsync()
        {
            var optionEntities = ThumbnailOptionEntitesDataSource.ToArray();
            var options = ThumbnailOptionDataSource.ToArray();

            var mock = new Mock<IThumbnailRepository>();
            mock.SetupGet(x => x.UnitOfWork).Returns(new Mock<IUnitOfWork>().Object);
            mock.Setup(r => r.GetThumbnailOptionsByIdsAsync(It.IsAny<string[]>()))
                .ReturnsAsync((string[] ids) =>
                {
                    var result = optionEntities.Where(t => ids.Contains(t.Id)).ToArray();
                    return result;
                });

            var sut = new ThumbnailOptionService(() => mock.Object);
            await sut.SaveOrUpdateAsync(options);

            Assert.Contains(optionEntities, o => o.Name == "New Name");
        }

        [Fact]
        public async Task SaveChanges_ArrayOfThumbnailOptions_NewThumbnailOptionsSaved()
        {
            var optionEntities = ThumbnailOptionEntitesDataSource.ToList();

            var mock = new Mock<IThumbnailRepository>();
            mock.SetupGet(x => x.UnitOfWork).Returns(new Mock<IUnitOfWork>().Object);
            mock.Setup(x => x.Add(It.IsAny<ThumbnailOptionEntity>()))
                .Callback((ThumbnailOptionEntity entity) =>
                {
                    optionEntities.Add(entity);
                });
            mock.Setup(r => r.GetThumbnailOptionsByIdsAsync(It.IsAny<string[]>()))
                .ReturnsAsync((string[] ids) =>
                {
                    return optionEntities.Where(t => ids.Contains(t.Id)).ToArray();
                });

            var sut = new ThumbnailOptionService(() => mock.Object);
            await sut.SaveOrUpdateAsync(new[]
            {
                new ThumbnailOption
                {
                    Id = "NewOptionId", Name = "New Option name"
                }
            });

            Assert.Contains(optionEntities, x => x.Id == "NewOptionId");
        }

        private static IEnumerable<ThumbnailOptionEntity> ThumbnailOptionEntitesDataSource
        {
            get
            {
                yield return new ThumbnailOptionEntity { Id = "Option 1" };
                yield return new ThumbnailOptionEntity { Id = "Option 2" };
                yield return new ThumbnailOptionEntity { Id = "Option 3" };
            }
        }

        private static IEnumerable<ThumbnailOption> ThumbnailOptionDataSource
        {
            get
            {
                yield return new ThumbnailOption { Id = "Option 1", Name = "New Name" };
                yield return new ThumbnailOption { Id = "Option 2", Name = "New Name" };
                yield return new ThumbnailOption { Id = "Option 3", Name = "New Name" };
            }
        }
    }
}
