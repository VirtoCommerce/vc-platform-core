using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
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
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace VirtoCommerce.CustomerModule.Tests
{
    public class CommerceMembersServiceImplUnitTests
    {
        private readonly Mock<ICustomerRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Func<ICustomerRepository> _repositoryFactory;
        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly Mock<IDynamicPropertyService> _dynamicPropertyServiceMock;
        private readonly Mock<ISeoService> _seoSericeMock;
        private readonly Mock<IPlatformMemoryCache> _platformMemoryCacheMock;
        private readonly Mock<ICacheEntry> _сacheEntryMock;
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
            _сacheEntryMock = new Mock<ICacheEntry>();
            _сacheEntryMock.SetupGet(c => c.ExpirationTokens).Returns(new List<IChangeToken>());
            _commerceMembersServiceImpl = new CommerceMembersServiceImpl(_repositoryFactory, _eventPublisherMock.Object, _dynamicPropertyServiceMock.Object, _seoSericeMock.Object, _platformMemoryCacheMock.Object);

            AbstractTypeFactory<Member>.RegisterType<Organization>().MapToType<OrganizationEntity>();
            AbstractTypeFactory<Member>.RegisterType<Contact>().MapToType<ContactEntity>();
            AbstractTypeFactory<Member>.RegisterType<Vendor>().MapToType<VendorEntity>();
            AbstractTypeFactory<Member>.RegisterType<Employee>().MapToType<EmployeeEntity>();

            AbstractTypeFactory<MemberEntity>.RegisterType<ContactEntity>();
            AbstractTypeFactory<MemberEntity>.RegisterType<OrganizationEntity>();
            AbstractTypeFactory<MemberEntity>.RegisterType<VendorEntity>();
            AbstractTypeFactory<MemberEntity>.RegisterType<EmployeeEntity>();
        }

        [Fact]
        public async Task GetByIdsAsync_GetMember()
        {
            //arrange
            var id = Guid.NewGuid().ToString();
            var ids = new[] {id};

            var member = AbstractTypeFactory<MemberEntity>.TryCreateInstance(nameof(EmployeeEntity));
            member.Id = id;
            member.Name = "TestEmployee";
            member.MemberType = nameof(Employee);
            var memberTypes = Enumerable.Empty<string>().ToArray();
            _repositoryMock.Setup(r => r.GetMembersByIdsAsync(ids, null, memberTypes)).ReturnsAsync(new [] {member});
            var cacheKey = CacheKey.With(_commerceMembersServiceImpl.GetType(), "GetByIdsAsync", string.Join("-", ids), null, string.Join("-", memberTypes));
            
            _platformMemoryCacheMock.Setup(pmc => pmc.CreateEntry(cacheKey)).Returns(_сacheEntryMock.Object);

            //act
            var members = await _commerceMembersServiceImpl.GetByIdsAsync(ids, null, memberTypes);

            //assert
            Assert.IsTrue(members.Any());
            Assert.IsTrue(members.Select(m => m.Id).Contains(id));
        }

        [Fact]
        public async Task SaveChangesAsync_CreateMember()
        {
            //arrange
            var id = Guid.NewGuid().ToString();
            var ids = new[] { id };
            var member = AbstractTypeFactory<Member>.TryCreateInstance(nameof(Employee));
            member.Name = "Test";
            member.Id = id;

            //act
            await _commerceMembersServiceImpl.SaveChangesAsync(new[] {member});

            //assert
        }

    }
}
