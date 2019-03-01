using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Web.Model
{
    /// <summary>
    /// Merchandising Category
    /// </summary>
    public class Category : AuditableEntity, ISeoSupport, IHasOutlines
    {
        /// <summary>
        /// Gets or sets the parent category id.
        /// </summary>
        /// <value>
        /// The parent identifier.
        /// </value>
        public string ParentId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Category"/> is virtual or common.
        /// </summary>
        /// <value>
        ///   <c>true</c> if virtual; otherwise, <c>false</c>.
        /// </value>
        public bool IsVirtual { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the type of the tax.
        /// </summary>
        /// <value>
        /// The type of the tax.
        /// </value>
		public string TaxType { get; set; }

        /// <summary>
        /// Gets or sets the catalog id that this category belongs to.
        /// </summary>
        /// <value>
        /// The catalog identifier.
        /// </value>
        public string CatalogId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Category outline in physical catalog (all parent categories ids concatenated. E.g. (1/21/344))
        /// </summary>
        public string Outline { get; set; }
        /// <summary>
        /// Category path in physical catalog (all parent categories names concatenated. E.g. (parent1/parent2))
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Category"/> is active.
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
		public ICollection<Property> Properties { get; set; }
        /// <summary>
        /// Gets or sets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
		public ICollection<CategoryLink> Links { get; set; }

        private string _imgSrc;
        /// <summary>
        /// Gets the default image for the category.
        /// </summary>
        /// <value>
        /// The image source URL.
        /// </value>
        public string ImgSrc
        {
            get
            {
                if (_imgSrc == null)
                {
                    if (Images != null && Images.Any())
                    {
                        _imgSrc = Images.First().Url;
                    }
                }
                return _imgSrc;
            }
        }

        /// <summary>
        /// Gets or sets the images.
        /// </summary>
        /// <value>
        /// The images.
        /// </value>
        public ICollection<Image> Images { get; set; }

        public string[] SecurityScopes { get; set; }

        #region ISeoSupport Members 
        public string SeoObjectType { get { return GetType().Name; } }
        /// <summary>
        /// Gets or sets the list of SEO information records.
        /// </summary>
        /// <value>
        /// The seo infos.
        /// </value>
        public IList<SeoInfo> SeoInfos { get; set; }
        #endregion

        #region Implementation of IHasOutlines

        public ICollection<Outline> Outlines { get; set; }

        #endregion
    }
}
