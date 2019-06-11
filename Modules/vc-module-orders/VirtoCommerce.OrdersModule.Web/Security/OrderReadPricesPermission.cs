using System.Linq;
using VirtoCommerce.OrdersModule.Core;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.OrderModule.Web.Security
{
    public static class OrderReadPricesPermission
    {
        public static string ApplyResponseGroupFiltering(Permission[] permissions, string respGroup)
        {
            var result = respGroup;

            var needRestrict = permissions.Any() && !permissions.Any(x => x.Id == ModuleConstants.Security.Permissions.ReadPrices);

            if (needRestrict && string.IsNullOrWhiteSpace(respGroup))
            {
                const CustomerOrderResponseGroup val = CustomerOrderResponseGroup.Full & ~CustomerOrderResponseGroup.WithPrices;

                result = val.ToString();
            }
            else if (needRestrict)
            {
                var items = respGroup.Split(',').Select(x => x.Trim()).ToList();

                items.Remove(CustomerOrderResponseGroup.WithPrices.ToString());

                result = string.Join(",", items);
            }

            if (needRestrict && string.IsNullOrEmpty(result))
            {
                result = CustomerOrderResponseGroup.Default.ToString();
            }

            return result;
        }
    }
}
