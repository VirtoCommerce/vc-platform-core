using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.SearchModule.Core.Model
{
    public class SearchConnection : ISearchConnection
    {
        private readonly IReadOnlyDictionary<string, string> _parameters;

        public SearchConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            _parameters = ParseString(connectionString);

            Provider = this["provider"];
            Scope = this["scope"];
        }

        public string Provider { get; }
        public string Scope { get; }


        public string this[string name] => _parameters.ContainsKey(name) ? _parameters[name] : null;

        private static IReadOnlyDictionary<string, string> ParseString(string input)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var vp in Regex.Split(input, ";"))
            {
                var singlePair = Regex.Split(vp, "=");
                result.Add(singlePair[0], singlePair.Length == 2 ? singlePair[1] : string.Empty);
            }

            return result;
        }
    }
}
