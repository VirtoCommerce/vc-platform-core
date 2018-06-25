using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.SearchModule.Data.Search.SearchPhraseParsing.Antlr;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.SearchModule.Data.Search.SearchPhraseParsing
{
    [CLSCompliant(false)]
    public class SearchPhraseListener : SearchPhraseBaseListener
    {
        public IList<string> Keywords { get; } = new List<string>();
        public IList<IFilter> Filters { get; } = new List<IFilter>();

        public override void ExitKeyword(Antlr.SearchPhraseParser.KeywordContext context)
        {
            base.ExitKeyword(context);
            Keywords.Add(Unescape(context.GetText()));
        }

        public override void ExitAttributeFilter(Antlr.SearchPhraseParser.AttributeFilterContext context)
        {
            base.ExitAttributeFilter(context);

            var fieldNameContext = context.GetChild<Antlr.SearchPhraseParser.FieldNameContext>(0);
            var attributeValueContext = context.GetChild<Antlr.SearchPhraseParser.AttributeFilterValueContext>(0);

            if (fieldNameContext != null && attributeValueContext != null)
            {
                var values = attributeValueContext.children.OfType<Antlr.SearchPhraseParser.StringContext>().ToArray();

                var filter = new TermFilter
                {
                    FieldName = Unescape(fieldNameContext.GetText()),
                    Values = values.Select(v => Unescape(v.GetText())).ToArray(),
                };

                Filters.Add(filter);
            }
        }

        public override void ExitRangeFilter(Antlr.SearchPhraseParser.RangeFilterContext context)
        {
            base.ExitRangeFilter(context);

            var fieldNameContext = context.GetChild<Antlr.SearchPhraseParser.FieldNameContext>(0);
            var rangeValueContext = context.GetChild<Antlr.SearchPhraseParser.RangeFilterValueContext>(0);

            if (fieldNameContext != null && rangeValueContext != null)
            {
                var values = rangeValueContext.children
                    .OfType<Antlr.SearchPhraseParser.RangeContext>()
                    .Select(GetRangeFilterValue)
                    .ToArray();

                var filter = new RangeFilter
                {
                    FieldName = Unescape(fieldNameContext.GetText()),
                    Values = values,
                };

                Filters.Add(filter);
            }
        }

        protected virtual RangeFilterValue GetRangeFilterValue(Antlr.SearchPhraseParser.RangeContext context)
        {
            var lower = context.GetChild<Antlr.SearchPhraseParser.LowerContext>(0)?.GetText();
            var upper = context.GetChild<Antlr.SearchPhraseParser.UpperContext>(0)?.GetText();
            var rangeStart = context.GetChild<Antlr.SearchPhraseParser.RangeStartContext>(0)?.GetText();
            var rangeEnd = context.GetChild<Antlr.SearchPhraseParser.RangeEndContext>(0)?.GetText();

            return new RangeFilterValue
            {
                Lower = Unescape(lower),
                Upper = Unescape(upper),
                IncludeLower = rangeStart.EqualsInvariant("["),
                IncludeUpper = rangeEnd.EqualsInvariant("]"),
            };
        }

        protected virtual string Unescape(string value)
        {
            return string.IsNullOrEmpty(value) ? value : value.Trim('"').Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\\\", "\\");
        }
    }
}
