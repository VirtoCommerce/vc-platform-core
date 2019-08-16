using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model.Conditions;
using VirtoCommerce.PricingModule.Core.Services;
using VirtoCommerce.PricingModule.Data.Model;
using VirtoCommerce.PricingModule.Data.Repositories;
using VirtoCommerce.PricingModule.Data.Services;
using Xunit;

namespace VirtoCommerce.PricingModule.Test
{
    public class PriceScenarios
    {
        [Fact]
        public async Task Can_return_pricelists()
        {
            RegisterTypes();
            var evalContext = new PriceEvaluationContext
            {
                ProductIds = new[] { "4ed55441810a47da88a483e5a1ee4e94" },
                UserGroups = new [] { "VIP" }
            };

            var pricingService = GetPricingService(GetPricingRepository);
            var priceLists = await pricingService.EvaluatePriceListsAsync(evalContext);
            Assert.True(priceLists.Any());
            var prices = await pricingService.EvaluateProductPricesAsync(evalContext);
            Assert.True(prices.Any());
        }

        [Fact]
        public async Task Can_return_prices_from_many_pricelists_by_priority()
        {
            RegisterTypes();
            var evalContext = new PriceEvaluationContext
            {
                ProductIds = new[] { "ProductId" },
                PricelistIds = new[] { "Pricelist 1", "Pricelist 2", "Pricelist 3" },
                Quantity = 0
            };

            var mockPrices = new Common.TestAsyncEnumerable<PriceEntity>(new List<PriceEntity> {
                new PriceEntity { List = 10, MinQuantity = 2, PricelistId = "Pricelist 1", Id = "1", ProductId = "ProductId" },

                new PriceEntity { List = 9, MinQuantity = 1, PricelistId = "Pricelist 2", Id = "2", ProductId = "ProductId" },
                new PriceEntity { List = 10, MinQuantity = 2, PricelistId = "Pricelist 2", Id = "3", ProductId = "ProductId" },

                new PriceEntity { List = 6, MinQuantity = 2, PricelistId = "Pricelist 3", Id = "4", ProductId = "ProductId" },
                new PriceEntity { List = 5, MinQuantity = 3,  PricelistId = "Pricelist 3", Id = "5", ProductId = "ProductId" }
            });

            var pricingService = GetPricingService(() => GetPricingRepositoryMock(mockPrices));

            var prices = (await pricingService.EvaluateProductPricesAsync(evalContext)).ToArray();

            // only 2 prices (from higher priority pricelists) returned, but not for MinQuantity == 3
            Assert.Equal(2, prices.Length);
            //Assert.Equal(mockPrices[1].Id, prices[0].Id);
            //Assert.Equal(mockPrices[0].Id, prices[1].Id);
            Assert.DoesNotContain(prices, x => x.MinQuantity == 3);

            // Pricelist priority changed
            evalContext.PricelistIds = new[] { "Pricelist 3", "Pricelist 2", "Pricelist 1" };
            prices = (await pricingService.EvaluateProductPricesAsync(evalContext)).ToArray();

            // 3 prices returned, but not from "Pricelist 1"
            Assert.Equal(3, prices.Length);
            //Assert.Equal(mockPrices[1].Id, prices[0].Id);
            //Assert.Equal(mockPrices[3].Id, prices[1].Id);
            //Assert.Equal(mockPrices[4].Id, prices[2].Id);
            Assert.DoesNotContain(prices, x => x.PricelistId == "Pricelist 1");
        }

        private IPricingService GetPricingService(Func<IPricingRepository> repositoryFactory)
        {
            var logger = new Moq.Mock<ILogger<PricingServiceImpl>>();
            var platformMemoryCache = new Mock<IPlatformMemoryCache>();
            var cacheEntry = new Mock<ICacheEntry>();
            cacheEntry.SetupGet(c => c.ExpirationTokens).Returns(new List<IChangeToken>());
            var cacheKey = CacheKey.With(typeof(PricingServiceImpl), "EvaluatePriceListsAsync");
            platformMemoryCache.Setup(pmc => pmc.CreateEntry(cacheKey)).Returns(cacheEntry.Object);
            return new PricingServiceImpl(repositoryFactory, null, logger.Object, platformMemoryCache.Object,  null);
        }

        private IPricingRepository GetPricingRepository()
        {
            var dbContextFactory = new DesignTimeDbContextFactory();
            var dbContext = dbContextFactory.CreateDbContext(new string[0]);

            var result = new PricingRepositoryImpl(dbContext);
            return result;
        }

        private IPricingRepository GetPricingRepositoryMock(IEnumerable<PriceEntity> prices)
        {
            var mock = new Moq.Mock<IPricingRepository>();
            mock.Setup(foo => foo.Prices).Returns(prices.AsQueryable());

            return mock.Object;
        }

        private void RegisterTypes()
        {
            if (AbstractTypeFactory<IConditionTree>.AllTypeInfos.All(t => t.Type != typeof(PriceConditionTree)))
                AbstractTypeFactory<IConditionTree>.RegisterType<PriceConditionTree>();

            if (AbstractTypeFactory<IConditionTree>.AllTypeInfos.All(t => t.Type != typeof(BlockPricingCondition)))
                AbstractTypeFactory<IConditionTree>.RegisterType<BlockPricingCondition>();

            if (AbstractTypeFactory<IConditionTree>.AllTypeInfos.All(t => t.Type != typeof(UserGroupsContainsCondition)))
                AbstractTypeFactory<IConditionTree>.RegisterType<UserGroupsContainsCondition>();
            

        }
    }


}
