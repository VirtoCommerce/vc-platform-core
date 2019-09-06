using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Data.Search.SearchPhraseParsing.Antlr;

namespace VirtoCommerce.SearchModule.Data.SearchPhraseParsing
{
    public class SearchPhraseListener : SearchPhraseBaseListener
    {
        public IList<string> Keywords { get; } = new List<string>();
        public IList<IFilter> Filters { get; } = new List<IFilter>();

        public override void ExitKeyword(Search.SearchPhraseParsing.Antlr.SearchPhraseParser.KeywordContext context)
        {
            base.ExitKeyword(context);
            Keywords.Add(UnEscape(context.GetText()));
        }

        public override void ExitAttributeFilter(Search.SearchPhraseParsing.Antlr.SearchPhraseParser.AttributeFilterContext context)
        {
            base.ExitAttributeFilter(context);

            var fieldNameContext = context.GetChild<Search.SearchPhraseParsing.Antlr.SearchPhraseParser.FieldNameContext>(0);
            var attributeValueContext = context.GetChild<Search.SearchPhraseParsing.Antlr.SearchPhraseParser.AttributeFilterValueContext>(0);

            if (fieldNameContext != null && attributeValueContext != null)
            {
                var values = attributeValueContext.children.OfType<Search.SearchPhraseParsing.Antlr.SearchPhraseParser.StringContext>().ToArray();

                var filter = new TermFilter
                {
                    FieldName = UnEscape(fieldNameContext.GetText()),
                    Values = values.Select(v => UnEscape(v.GetText())).ToArray(),
                };

                Filters.Add(filter);
            }
        }

        public override void ExitRangeFilter(Search.SearchPhraseParsing.Antlr.SearchPhraseParser.RangeFilterContext context)
        {
            base.ExitRangeFilter(context);

            var fieldNameContext = context.GetChild<Search.SearchPhraseParsing.Antlr.SearchPhraseParser.FieldNameContext>(0);
            var rangeValueContext = context.GetChild<Search.SearchPhraseParsing.Antlr.SearchPhraseParser.RangeFilterValueContext>(0);

            if (fieldNameContext != null && rangeValueContext != null)
            {
                var values = rangeValueContext.children
                    .OfType<Search.SearchPhraseParsing.Antlr.SearchPhraseParser.RangeContext>()
                    .Select(GetRangeFilterValue)
                    .ToArray();

                var filter = new RangeFilter
                {
                    FieldName = UnEscape(fieldNameContext.GetText()),
                    Values = values,
                };

                Filters.Add(filter);
            }
        }

        protected virtual RangeFilterValue GetRangeFilterValue(Search.SearchPhraseParsing.Antlr.SearchPhraseParser.RangeContext context)
        {
            var lower = context.GetChild<Search.SearchPhraseParsing.Antlr.SearchPhraseParser.LowerContext>(0)?.GetText();
            var upper = context.GetChild<Search.SearchPhraseParsing.Antlr.SearchPhraseParser.UpperContext>(0)?.GetText();
            var rangeStart = context.GetChild<Search.SearchPhraseParsing.Antlr.SearchPhraseParser.RangeStartContext>(0)?.GetText();
            var rangeEnd = context.GetChild<Search.SearchPhraseParsing.Antlr.SearchPhraseParser.RangeEndContext>(0)?.GetText();

            return new RangeFilterValue
            {
                Lower = UnEscape(lower),
                Upper = UnEscape(upper),
                IncludeLower = rangeStart.EqualsInvariant("["),
                IncludeUpper = rangeEnd.EqualsInvariant("]"),
            };
        }

        protected virtual string UnEscape(string value)
        {
            return string.IsNullOrEmpty(value) ? value : value.Trim('"').Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\\\", "\\");
        }
    }
}
