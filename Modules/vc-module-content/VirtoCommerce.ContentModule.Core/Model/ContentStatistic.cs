using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Core.Model
{
    /// <summary>
    /// Represent a summary content statistics 
    /// </summary>
    public class ContentStatistic : ValueObject
    {
        public string ActiveThemeName { get; set; }
        public int ThemesCount { get; set; }
        public int PagesCount { get; set; }
        public int BlogsCount { get; set; }
    }
}
