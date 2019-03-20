using System.Linq;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.SearchModule.Data.SearchPhraseParsing;
using Xunit;

namespace VirtoCommerce.SearchModule.Tests
{
    [Trait("Category", "Unit")]
    public class SearchPhraseParserTests
    {
        [Fact]
        public void TestKeywords()
        {
            var parser = Getparser();
            var result = parser.Parse(" one two three ");

            Assert.NotNull(result);
            Assert.Equal("one two three", result.Keyword);
            Assert.Empty(result.Filters);
        }

        [Fact]
        public void TestNegationFilter()
        {
            var parser = Getparser();
            var result = parser.Parse("!size:medium");

            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Keyword);
            Assert.NotNull(result.Filters);
            Assert.Equal(1, result.Filters.Count);

            var filter = result.Filters.First() as NotFilter;
            Assert.NotNull(filter);
            Assert.NotNull(filter.ChildFilter);
        }

        [Fact]
        public void TestAttributeFilter()
        {
            var parser = Getparser();
            var result = parser.Parse("color:red,blue");

            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Keyword);
            Assert.NotNull(result.Filters);
            Assert.Equal(1, result.Filters.Count);

            var filter = result.Filters.First() as TermFilter;
            Assert.NotNull(filter);
            Assert.Equal("color", filter.FieldName);
            Assert.NotNull(filter.Values);
            Assert.Equal(2, filter.Values.Count);

            var value = filter.Values.First();
            Assert.NotNull(value);
            Assert.Equal("red", value);

            value = filter.Values.Last();
            Assert.NotNull(value);
            Assert.Equal("blue", value);
        }

        [Fact]
        public void TestRangeFilter()
        {
            var parser = Getparser();
            var result = parser.Parse("size:(10 TO 20],[30 to 40)");

            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Keyword);
            Assert.NotNull(result.Filters);
            Assert.Equal(1, result.Filters.Count);

            var filter = result.Filters.First() as RangeFilter;
            Assert.NotNull(filter);
            Assert.Equal("size", filter.FieldName);
            Assert.NotNull(filter.Values);
            Assert.Equal(2, filter.Values.Count);

            var value = filter.Values.First();
            Assert.NotNull(value);
            Assert.Equal("10", value.Lower);
            Assert.Equal("20", value.Upper);
            Assert.False(value.IncludeLower);
            Assert.True(value.IncludeUpper);

            value = filter.Values.Last();
            Assert.NotNull(value);
            Assert.Equal("30", value.Lower);
            Assert.Equal("40", value.Upper);
            Assert.True(value.IncludeLower);
            Assert.False(value.IncludeUpper);


            result = parser.Parse("size:(TO 10]");

            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Keyword);
            Assert.NotNull(result.Filters);
            Assert.Equal(1, result.Filters.Count);

            filter = result.Filters.First() as RangeFilter;

            Assert.NotNull(filter);
            Assert.Equal("size", filter.FieldName);
            Assert.NotNull(filter.Values);
            Assert.Equal(1, filter.Values.Count);

            value = filter.Values.First();
            Assert.NotNull(value);
            Assert.Null(value.Lower);
            Assert.Equal("10", value.Upper);
            Assert.False(value.IncludeLower);
            Assert.True(value.IncludeUpper);


            result = parser.Parse("size:(10 TO]");

            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Keyword);
            Assert.NotNull(result.Filters);
            Assert.Equal(1, result.Filters.Count);

            filter = result.Filters.First() as RangeFilter;

            Assert.NotNull(filter);
            Assert.Equal("size", filter.FieldName);
            Assert.NotNull(filter.Values);
            Assert.Equal(1, filter.Values.Count);

            value = filter.Values.First();
            Assert.NotNull(value);
            Assert.Equal("10", value.Lower);
            Assert.Null(value.Upper);
            Assert.False(value.IncludeLower);
            Assert.True(value.IncludeUpper);
        }

        [Fact]
        public void TestKeywordAndFilter()
        {
            var parser = Getparser();
            var result = parser.Parse("one brand:apple two");

            Assert.NotNull(result);
            Assert.Equal("one two", result.Keyword);
            Assert.NotNull(result.Filters);
            Assert.Equal(1, result.Filters.Count);

            var filter = result.Filters.First() as TermFilter;

            Assert.NotNull(filter);

            Assert.NotNull(filter);
            Assert.Equal("brand", filter.FieldName);
            Assert.NotNull(filter.Values);
            Assert.Equal(1, filter.Values.Count);

            var value = filter.Values.First();
            Assert.NotNull(value);
            Assert.Equal("apple", value);
        }

        [Fact]
        public void TestQuotedStrings()
        {
            var parser = Getparser();

            // Keywords
            var result = parser.Parse("one \"two \\r\\n\\t\\\\ three\" four");

            Assert.NotNull(result);
            Assert.Equal("one two \r\n\t\\ three four", result.Keyword);
            Assert.Empty(result.Filters);


            // Attribute filter
            result = parser.Parse("\"color \\r\\n\\t\\\\ 2\":\"light \\r\\n\\t\\\\ blue\"");

            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Keyword);
            Assert.NotNull(result.Filters);
            Assert.Equal(1, result.Filters.Count);

            var filter = result.Filters.First() as TermFilter;
            Assert.NotNull(filter);
            Assert.Equal("color \r\n\t\\ 2", filter.FieldName);
            Assert.NotNull(filter.Values);
            Assert.Equal(1, filter.Values.Count);

            var value = filter.Values.First();
            Assert.NotNull(value);
            Assert.Equal("light \r\n\t\\ blue", value);


            // Range filter
            result = parser.Parse("date:[\"2017-04-23T15:24:31.180Z\" to \"2017-04-28T15:24:31.180Z\"]");

            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Keyword);
            Assert.NotNull(result.Filters);
            Assert.Equal(1, result.Filters.Count);

            var rangeFilter = result.Filters.Last() as RangeFilter;
            Assert.NotNull(rangeFilter);
            Assert.Equal("date", rangeFilter.FieldName);
            Assert.NotNull(rangeFilter.Values);
            Assert.Equal(1, rangeFilter.Values.Count);

            var rangeValue = rangeFilter.Values.First();
            Assert.NotNull(value);
            Assert.Equal("2017-04-23T15:24:31.180Z", rangeValue.Lower);
            Assert.Equal("2017-04-28T15:24:31.180Z", rangeValue.Upper);
            Assert.True(rangeValue.IncludeLower);
            Assert.True(rangeValue.IncludeUpper);
        }


        private static ISearchPhraseParser Getparser()
        {
            return new SearchPhraseParser();
        }
    }
}
