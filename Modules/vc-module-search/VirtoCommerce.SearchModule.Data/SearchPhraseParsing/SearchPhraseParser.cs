using VirtoCommerce.SearchModule.Data.Search.SearchPhraseParsing.Antlr;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.SearchModule.Core.Model;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace VirtoCommerce.SearchModule.Data.Search.SearchPhraseParsing
{
    public class SearchPhraseParser : ISearchPhraseParser
    {
        public virtual SearchPhraseParseResult Parse(string input)
        {
            var stream = CharStreams.fromstring(input);
            var lexer = new SearchPhraseLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new Antlr.SearchPhraseParser(tokens) { BuildParseTree = true };
            var listener = new SearchPhraseListener();

            ParseTreeWalker.Default.Walk(listener, parser.searchPhrase());

            var result = new SearchPhraseParseResult
            {
                SearchPhrase = string.Join(" ", listener.Keywords),
                Filters = listener.Filters,
            };

            return result;
        }
    }
}
