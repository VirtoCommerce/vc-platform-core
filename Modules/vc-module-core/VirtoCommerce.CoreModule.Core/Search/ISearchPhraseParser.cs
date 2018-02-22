namespace VirtoCommerce.Domain.Search
{
    public interface ISearchPhraseParser
    {
        SearchPhraseParseResult Parse(string input);
    }
}
