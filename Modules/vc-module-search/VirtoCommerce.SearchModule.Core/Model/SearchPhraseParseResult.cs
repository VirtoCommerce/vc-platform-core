using System.Collections.Generic;

namespace VirtoCommerce.SearchModule.Core.Model
{
    public class SearchPhraseParseResult
    {
        public string Keyword { get; set; }
        public IList<IFilter> Filters { get; set; }
    }
}
