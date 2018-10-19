using System;

namespace VirtoCommerce.PricingModule.Core.Model.Promotions.Rewards
{
    public static class MoneyExtensions
    {
        public static decimal[] Allocate(this decimal amount, int qty, int decimalDigits = 2)
        {
            var cents = Math.Pow(10, decimalDigits);
            var lowResult = ((long)Math.Truncate((double)amount / qty * cents)) / cents;
            var highResult = lowResult + 1.0d / cents;
            var remainder = (int)(((double)amount * cents) % qty);

            var results = new decimal[qty];

            for (var i = 0; i < remainder; i++)
            {
                results[i] = (decimal)highResult;
            }
            for (var i = remainder; i < qty; i++)
            {
                results[i] = (decimal)lowResult;
            }
            return results;
        }
    }
}
