using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrdersModule.Core.Model
{
    public class Address : CoreModule.Core.Common.Address
	{
        //Temporary workaround to be able make references to the address
        public string Key { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            var result = base.GetEqualityComponents();
            if (!string.IsNullOrEmpty(Key))
            {
                result = new[] { Key };
            }
            return result;
        }
    }

}
