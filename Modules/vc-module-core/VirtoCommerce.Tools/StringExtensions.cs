using System;

namespace VirtoCommerce.Tools
{
    internal static class StringExtensions
    {
        internal static bool EqualsInvariant(this string str1, string str2)
        {
            return string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
        }
    }
}
