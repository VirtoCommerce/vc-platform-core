using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Data.Model;
using VirtoCommerce.CustomerModule.Data.Repositories;
using VirtoCommerce.CustomerModule.Data.Services;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CustomerModule.Tests
{
    class CommerceMembersServiceImplUnitTests
    {
        private readonly Mock<ICustomerRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Func<ICustomerRepository> _repositoryFactory;
        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly Mock<IDynamicPropertyService> _dynamicPropertyServiceMock;
        private readonly Mock<ISeoService> _seoSericeMock;
        private readonly Mock<IPlatformMemoryCache> _platformMemoryCacheMock;
        private readonly CommerceMembersServiceImpl _commerceMembersServiceImpl;

        public CommerceMembersServiceImplUnitTests()
        {
            _repositoryMock = new Mock<ICustomerRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _eventPublisherMock = new Mock<IEventPublisher>();
            _dynamicPropertyServiceMock = new Mock<IDynamicPropertyService>();
            _seoSericeMock = new Mock<ISeoService>();
            _repositoryMock.Setup(ss => ss.UnitOfWork).Returns(_mockUnitOfWork.Object);
            _repositoryFactory = () => _repositoryMock.Object;
            _platformMemoryCacheMock = new Mock<IPlatformMemoryCache>();
            _commerceMembersServiceImpl = new CommerceMembersServiceImpl(_repositoryFactory, _eventPublisherMock.Object, _dynamicPropertyServiceMock.Object, _seoSericeMock.Object, _platformMemoryCacheMock.Object);
        }

        public async Task GetByIdsAsync_GetMember()
        {
            //arrange
            var id = Guid.NewGuid().ToString();
            var ids = new[] {id};

            var member = FakeMemberByType(nameof(Employee));
            member.Id = id;
            _repositoryMock.Setup(r => r.GetMembersByIdsAsync(ids, null, null)).ReturnsAsync(new [] {member});

            //act
            var members = await _commerceMembersServiceImpl.GetByIdsAsync(ids);

            //assert
            Assert.IsTrue(members.Any());
            Assert.AreEqual(ids, members.Select(m => m.Id));
        }

        private MemberEntity FakeMemberByType(string type)
        {
            var member = AbstractTypeFactory<MemberEntity>.TryCreateInstance(type);
            member.Name = "TestMember";

            return member;
        }
    }
}
