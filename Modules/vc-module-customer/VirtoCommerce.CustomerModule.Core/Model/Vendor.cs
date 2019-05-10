using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Seo;

namespace VirtoCommerce.CustomerModule.Core.Model
{
    public class Vendor : Member, ISeoSupport
    {
        /// <summary>
        /// Vendor description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Vendor site url
        /// </summary>
        public string SiteUrl { get; set; }

        /// <summary>
        /// Vendor logo url
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// Vendor group
        /// </summary>
        public string GroupName { get; set; }

    }
}
