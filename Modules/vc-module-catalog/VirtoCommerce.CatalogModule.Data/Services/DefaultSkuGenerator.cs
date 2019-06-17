using System;
using System.Text;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;

namespace VirtoCommerce.CatalogModule.Data.Services
{
    /// <summary>
    /// XXX(leter)-XXXXXXXX(number).
    /// </summary>
    public class DefaultSkuGenerator : ISkuGenerator
    {
        private static readonly Random _random = new Random();
        private static readonly object _lockObject = new object();

        #region ISkuGenerator Members

        public string GenerateSku(CatalogProduct product)
        {
            const string leterPart = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digitPart = "1234567890";
            var res = new StringBuilder();

            lock (_lockObject)
            {
                for (var i = 0; i < 3; i++)
                {
                    res.Append(leterPart[_random.Next(leterPart.Length)]);
                }
                res.Append("-");
                for (var i = 0; i < 8; i++)
                {
                    res.Append(digitPart[_random.Next(digitPart.Length)]);
                }
            }
            return res.ToString();
        }

        #endregion
    }
}
