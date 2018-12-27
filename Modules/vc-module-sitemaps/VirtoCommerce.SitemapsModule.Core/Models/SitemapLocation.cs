using System;
using System.IO;
using System.Linq;

namespace VirtoCommerce.SitemapsModule.Core.Models
{
    public class SitemapLocation
    {
        public SitemapLocation(string location, int pageNumber, string pageNumberSeparator)
        {
            Location = location;
            PageNumber = pageNumber;
            PageNumberSeparator = pageNumberSeparator;
        }

        public string PageNumberSeparator { get; }
        public string Location { get; }
        public int PageNumber { get; }

        public string ToString(bool withPageNumber)
        {
            var retVal = Location;
            if(withPageNumber)
            {
                retVal = string.Format("{0}{1}{2}.xml", Location.Replace(".xml", string.Empty), PageNumberSeparator, PageNumber);
            }
            return retVal;
        }

        /// <summary>
        /// This methods parsed incoming url with format {sitemap-location}--{page-number}.xml
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pageNumberSeparator"></param>
        /// <returns></returns>
        public static SitemapLocation Parse(string url, string pageNumberSeparator = "--")
        {
            var location = url;
            int pageNumber = 1;
            if(location.Contains(pageNumberSeparator))
            {
                var parts = url.Split(new[] { pageNumberSeparator }, StringSplitOptions.RemoveEmptyEntries);
                location = parts.First() + ".xml";
                pageNumber = Convert.ToInt32(Path.GetFileNameWithoutExtension(parts[1]));
                 
            }
            return new SitemapLocation(location, pageNumber, pageNumberSeparator);
        }

     }
}
