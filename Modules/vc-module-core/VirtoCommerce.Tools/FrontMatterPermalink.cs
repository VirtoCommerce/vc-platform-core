using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace VirtoCommerce.Tools
{
    /// <summary>
    /// http://jekyllrb.com/docs/permalinks/
    /// </summary>
    public class FrontMatterPermalink
    {
        protected static readonly Regex TimestampAndTitleFromPathRegex = new Regex(string.Format(@"{0}(?:(?<timestamp>\d+-\d+-\d+)-)?(?<title>[^{0}]*)\.[^\.]+$", Regex.Escape("/")), RegexOptions.Compiled);
        protected static readonly Regex TimestampAndTitleAndLanguageFromPathRegex = new Regex(string.Format(@"{0}(?:(?<timestamp>\d+-\d+-\d+)-)?(?<title>[^{0}]*)\.(?<language>[A-z]{{2}}-[A-z]{{2}})\.[^\.]+$", Regex.Escape("/")), RegexOptions.Compiled);
        protected static readonly Regex CategoryRegex = new Regex(@":category(\d*)", RegexOptions.Compiled);
        protected static readonly Regex SlashesRegex = new Regex(@"/{1,}", RegexOptions.Compiled);

        //http://jekyllrb.com/docs/permalinks/#builtinpermalinkstyles
        protected static readonly Dictionary<string, string> BuiltInPermalinkStyles = new Dictionary<string, string>
        {
            { "date", ":folder/:categories/:year/:month/:day/:title" },
            { "pretty", ":folder/:categories/:year/:month/:day/:title/" },
            { "ordinal", ":folder/:categories/:year/:y_day/:title" },
            { "default", ":folder/:categories/:title" },
        };

        public FrontMatterPermalink()
        {
        }

        public FrontMatterPermalink(string urlTemplate)
        {
            UrlTemplate = urlTemplate;
        }

        //template-variable pattern.
        public string UrlTemplate { get; set; }
        public string FilePath { get; set; }
        public DateTime? Date { get; set; }
        public IList<string> Categories { get; set; }

        /// <summary>
        /// Build relative URL based on permalink template and other properties
        /// </summary>
        /// <returns></returns>
        public virtual string ToUrl()
        {
            var result = UrlTemplate?.Replace("~/", string.Empty) ?? string.Empty;

            if (BuiltInPermalinkStyles.ContainsKey(result))
            {
                result = BuiltInPermalinkStyles[result];
            }

            var removeLeadingSlash = !result.StartsWith("/");
            var categories = Categories ?? new string[] { };

            result = result.Replace(":folder", FilePath != null ? Path.GetDirectoryName(FilePath).Replace("\\", "/") : string.Empty);
            result = result.Replace(":categories", string.Join("/", categories));
            result = result.Replace(":dashcategories", string.Join("-", categories));
            result = result.Replace(":year", Date?.Year.ToString(CultureInfo.InvariantCulture) ?? string.Empty);
            result = result.Replace(":month", Date?.ToString("MM") ?? string.Empty);
            result = result.Replace(":day", Date?.ToString("dd") ?? string.Empty);
            result = result.Replace(":title", FilePath != null ? GetTitleFromFilePath(FilePath) : string.Empty);
            result = result.Replace(":y_day", Date?.DayOfYear.ToString("000") ?? string.Empty);
            result = result.Replace(":short_year", Date?.ToString("yy") ?? string.Empty);
            result = result.Replace(":i_month", Date?.Month.ToString() ?? string.Empty);
            result = result.Replace(":i_day", Date?.Day.ToString() ?? string.Empty);

            if (result.Contains(":category"))
            {
                var matches = CategoryRegex.Matches(result);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        var replacementValue = string.Empty;
                        if (match.Success)
                        {
                            int categoryIndex;
                            if (int.TryParse(match.Groups[1].Value, out categoryIndex) && categoryIndex > 0)
                            {
                                replacementValue = categories.Skip(categoryIndex - 1).FirstOrDefault();
                            }
                            else if (categories.Any())
                            {
                                replacementValue = categories.First();
                            }
                        }

                        result = result.Replace(match.Value, replacementValue);
                    }
                }
            }

            result = SlashesRegex.Replace(result, "/");

            if (removeLeadingSlash)
                result = result.TrimStart('/');

            return result;
        }

        protected virtual string GetTitleFromFilePath(string filePath)
        {
            // Try to extract title with language
            var title = TimestampAndTitleAndLanguageFromPathRegex.Match(filePath).Groups["title"].Value;

            // Try to extract title without language
            if (string.IsNullOrEmpty(title))
                title = TimestampAndTitleFromPathRegex.Match(filePath).Groups["title"].Value;

            return title;
        }
    }
}
