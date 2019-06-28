using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;
using Xunit;

namespace VirtoCommerce.CartModule.Test.UnitTests
{
    public class ShoppingCartServiceImplUnitTests
    {
        private readonly Mock<IDynamicPropertyService> _dynamicPropertyServiceMock;
        private readonly Mock<IShoppingCartTotalsCalculator> _calculatorMock;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly Func<ICartRepository> _repositoryFactory;
        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly Mock<IPlatformMemoryCache> _platformMemoryCacheMock;
        private readonly Mock<ICacheEntry> _сacheEntryMock;
        private readonly ShoppingCartServiceImpl _shoppingCartServiceImpl;

        public ShoppingCartServiceImplUnitTests()
        {
            _dynamicPropertyServiceMock = new Mock<IDynamicPropertyService>();
            _calculatorMock = new Mock<IShoppingCartTotalsCalculator>();
            _cartRepositoryMock = new Mock<ICartRepository>();
            _repositoryFactory = () => _cartRepositoryMock.Object;
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _cartRepositoryMock.Setup(ss => ss.UnitOfWork).Returns(_mockUnitOfWork.Object);
            _eventPublisherMock = new Mock<IEventPublisher>();
            _platformMemoryCacheMock = new Mock<IPlatformMemoryCache>();
            _сacheEntryMock = new Mock<ICacheEntry>();
            _shoppingCartServiceImpl = new ShoppingCartServiceImpl(_repositoryFactory
                , _calculatorMock.Object
                , _eventPublisherMock.Object
                , _platformMemoryCacheMock.Object
                , null
                );
        }

        [Fact]
        public async Task GetByIdsAsync_ReturnCart()
        {
            //Arrange
            var cartId = Guid.NewGuid().ToString();
            var cartIds = new[] { cartId };
            var list = new List<ShoppingCartEntity>() { new ShoppingCartEntity() { Id = cartId } };
            _cartRepositoryMock.Setup(n => n.GetShoppingCartsByIdsAsync(cartIds, null))
                .ReturnsAsync(list.ToArray());
            var cacheKey = CacheKey.With(_shoppingCartServiceImpl.GetType(), "GetByIdsAsync", string.Join("-", cartIds), null);
            _platformMemoryCacheMock.Setup(pmc => pmc.CreateEntry(cacheKey)).Returns(_сacheEntryMock.Object);

            //Act
            var result = await _shoppingCartServiceImpl.GetByIdsAsync(new[] { cartId });

            //Assert
            Assert.True(result.Any());
            Assert.Contains(result, cart => cart.Id.Equals(cartId));
        }

        [Fact]
        public async Task SaveChangesAsync_CreateCart()
        {
            //Arrange
            var cartId = Guid.NewGuid().ToString();
            var entity = new ShoppingCartEntity() { Id = cartId };
            var carts = new List<ShoppingCart>() { entity.ToModel(AbstractTypeFactory<ShoppingCart>.TryCreateInstance()) };

            //Act
            await _shoppingCartServiceImpl.SaveChangesAsync(carts.ToArray());

            //Assert
        }

        [Fact]
        public async Task SaveChangesAsync_SaveCart()
        {
            //Arrange
            var cartId = Guid.NewGuid().ToString();
            var cartIds = new[] { cartId };
            var entity = new ShoppingCartEntity() { Id = cartId };
            var list = new List<ShoppingCartEntity>() { entity };
            _cartRepositoryMock.Setup(n => n.GetShoppingCartsByIdsAsync(cartIds, null))
                .ReturnsAsync(list.ToArray());
            var carts = new List<ShoppingCart>() { entity.ToModel(AbstractTypeFactory<ShoppingCart>.TryCreateInstance()) };

            //Act
            await _shoppingCartServiceImpl.SaveChangesAsync(carts.ToArray());

            //Assert
        }

    }
}
