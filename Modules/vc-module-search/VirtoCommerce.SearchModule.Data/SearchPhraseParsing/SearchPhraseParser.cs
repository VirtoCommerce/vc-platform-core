using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.SearchModule.Data.Search.SearchPhraseParsing;
using VirtoCommerce.SearchModule.Data.Search.SearchPhraseParsing.Antlr;

namespace VirtoCommerce.SearchModule.Data.SearchPhraseParsing
{
    public class SearchPhraseParser : ISearchPhraseParser
    {
        public virtual SearchPhraseParseResult Parse(string input)
        {
            var stream = CharStreams.fromstring(input);
            var lexer = new SearchPhraseLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new Search.SearchPhraseParsing.Antlr.SearchPhraseParser(tokens) { BuildParseTree = true };
            var listener = new SearchPhraseListener();

            ParseTreeWalker.Default.Walk(listener, parser.searchPhrase());

            var result = new SearchPhraseParseResult
            {
                Keyword = string.Join(" ", listener.Keywords),
                Filters = listener.Filters,
            };

            return result;
        }
    }
}
