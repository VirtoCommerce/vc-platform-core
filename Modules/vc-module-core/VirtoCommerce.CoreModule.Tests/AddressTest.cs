using VirtoCommerce.CoreModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Model.Tax;
using Xunit;

namespace VirtoCommerce.CoreModule.Tests
{
    public class AddressTest
    {

        [Fact]
        public virtual void AddressEquality_ShouldIgnore_Key()
        {
            var address1 = new Address
            {
                AddressType = AddressType.Billing,
                City = "City",
                CountryCode = "CountryCode",
                CountryName = "CountryName",
                Email = "email@mail.com",
                FirstName = "FirstName",
                LastName = "LastName",
                Line1 = "Line1",
                Line2 = "Line2",
                MiddleName = "MiddleName",
                Name = "Name",
                Organization = "Organization",
                Phone = "Phone",
                PostalCode = "PostalCode",
                RegionId = "RegionId",
                RegionName = "RegionName",
                Zip = "Zip"
            };
            var address2 = address1.Clone() as Address;

            Assert.Equal(address1, address2);
            address2.City = "City2";
            Assert.NotEqual(address1, address2);
        }        
    }
}
