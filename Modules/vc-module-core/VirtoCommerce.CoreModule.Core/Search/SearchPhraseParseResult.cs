using System.Collections.Generic;

namespace VirtoCommerce.Domain.Search
{
    public class SearchPhraseParseResult
    {
        public string SearchPhrase { get; set; }
        public IList<IFilter> Filters { get; set; }
    }
}
