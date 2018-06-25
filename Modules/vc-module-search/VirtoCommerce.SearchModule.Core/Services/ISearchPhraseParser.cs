using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.SearchModule.Core.Services
{
    public interface ISearchPhraseParser
    {
        SearchPhraseParseResult Parse(string input);
    }
}
