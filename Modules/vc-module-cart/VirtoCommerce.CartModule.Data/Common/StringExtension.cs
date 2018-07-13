using System;
using System.Linq;

namespace VirtoCommerce.CartModule.Data.Common
{
    public static class CartStringExtension
    {
        public static Tuple<string, string> SplitIntoTuple(this string input, char separator)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var pieces = input.Split(separator);
            return Tuple.Create(pieces.FirstOrDefault(), pieces.Skip(1).FirstOrDefault());
        }
    }
}
