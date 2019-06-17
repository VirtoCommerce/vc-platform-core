using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Common
{
    public class Address : ValueObject
    {
        public AddressType AddressType { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Organization { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Zip { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string RegionId { get; set; }
        public string RegionName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            var result = base.GetEqualityComponents();
            if (!string.IsNullOrEmpty(Key))
            {
                result = new[] { Key };
            }
            return result;
        }

        public override string ToString()
        {
            return string.Join(", ", new[] { FirstName, LastName, Line1, City, RegionName, PostalCode ?? Zip, CountryName }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }
    }

}
