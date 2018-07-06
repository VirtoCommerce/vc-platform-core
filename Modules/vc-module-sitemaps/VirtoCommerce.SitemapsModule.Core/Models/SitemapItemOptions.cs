using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.SitemapsModule.Core.Models
{
    public class SitemapItemOptions
    {
        public SitemapItemOptions()
        {
            Priority = .5M;
            UpdateFrequency = Models.UpdateFrequency.Weekly;
        }

        public string UpdateFrequency { get; set; }

        public decimal Priority { get; set; }
    }
}
