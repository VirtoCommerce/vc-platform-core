using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ContentModule.Core.Model
{
    public class FrontMatterHeaders : Entity, IHasDynamicProperties, ICloneable
    {
        #region IHasDynamicProperties Members

        public string ObjectType => typeof(FrontMatterHeaders).FullName;
        public ICollection<DynamicObjectProperty> DynamicProperties { get; set; }

        #endregion

        #region DynamicProperty

        //https://jekyllrb.com/docs/frontmatter/
        //Register special ContentItem.FrontMatterHeaders type which will be used to define YAML headers for pages, blogs and posts
        private static string frontMatterHeaderType = typeof(FrontMatterHeaders).FullName;

        //Title
        public static DynamicProperty titleHeader = new DynamicProperty
        {
            Id = "Title_FrontMatterHeader",
            Name = "title",
            ObjectType = frontMatterHeaderType,
            ValueType = DynamicPropertyValueType.ShortText,
            CreatedBy = "Auto"
        };

        //If set, this specifies the layout file to use. Use the layout file name without the file extension. 
        public static DynamicProperty layoutHeader = new DynamicProperty
        {
            Id = "Layout_FrontMatterHeader",
            Name = "layout",
            ObjectType = frontMatterHeaderType,
            ValueType = DynamicPropertyValueType.ShortText,
            CreatedBy = "Auto"
        };

        //Name of liquid template in theme witch will be used for display this page by including  {{ page.content }} expression 
        public static DynamicProperty templateHeader = new DynamicProperty
        {
            Id = "Template_FrontMatterHeader",
            Name = "template",
            ObjectType = frontMatterHeaderType,
            ValueType = DynamicPropertyValueType.ShortText,
            CreatedBy = "Auto"
        };

        //If you need your processed blog post URLs to be something other than the site-wide style (default /year/month/day/title.html), then you can set this variable and it will be used as the final URL.
        public static DynamicProperty permalinkHeader = new DynamicProperty
        {
            Id = "Permalink_FrontMatterHeader",
            Name = "permalink",
            ObjectType = frontMatterHeaderType,
            ValueType = DynamicPropertyValueType.ShortText,
            CreatedBy = "Auto"
        };

        //Set to false if you don’t want a specific post to show up when the site is generated.
        public static DynamicProperty publishedHeader = new DynamicProperty
        {
            Id = "Published_FrontMatterHeader",
            Name = "published",
            ObjectType = frontMatterHeaderType,
            ValueType = DynamicPropertyValueType.Boolean,
            CreatedBy = "Auto"
        };

        //Instead of placing posts inside of folders, you can specify one or more categories that the post belongs to. When the site is generated the post will act as though it had been set with these categories normally. Categories (plural key) can be specified as a YAML list or a comma-separated string.
        public static DynamicProperty categoryHeader = new DynamicProperty
        {
            Id = "Category_FrontMatterHeader",
            Name = "category",
            ObjectType = frontMatterHeaderType,
            ValueType = DynamicPropertyValueType.ShortText,
            CreatedBy = "Auto"
        };

        public static DynamicProperty categoriesHeader = new DynamicProperty
        {
            Id = "Categories_FrontMatterHeader",
            Name = "categories",
            IsArray = true,
            ObjectType = frontMatterHeaderType,
            ValueType = DynamicPropertyValueType.ShortText,
            CreatedBy = "Auto"
        };

        //Similar to categories, one or multiple tags can be added to a post. Also like categories, tags can be specified as a YAML list or a comma-separated string.
        public static DynamicProperty tagsHeader = new DynamicProperty
        {
            Id = "Tags_FrontMatterHeader",
            Name = "tags",
            IsArray = true,
            ObjectType = frontMatterHeaderType,
            ValueType = DynamicPropertyValueType.ShortText,
            CreatedBy = "Auto"
        };

        //It allows  to specify aliases on a per page basis, in the YAML front matter. This aliases can be used for redirection.
        public static DynamicProperty aliasesHeader = new DynamicProperty
        {
            Id = "Alias_FrontMatterHeader",
            Name = "aliases",
            ObjectType = frontMatterHeaderType,
            ValueType = DynamicPropertyValueType.ShortText,
            IsArray = true,
            CreatedBy = "Auto"
        };

        public static DynamicProperty isStickedHeader = new DynamicProperty
        {
            Id = "isSticked_FrontMatterHeader",
            Name = "is-sticked",
            ObjectType = frontMatterHeaderType,
            ValueType = DynamicPropertyValueType.Boolean,
            CreatedBy = "Auto"
        };

        public static DynamicProperty mainImageHeader = new DynamicProperty
        {
            Id = "mainImage_FrontMatterHeader",
            Name = "main-image",
            ObjectType = frontMatterHeaderType,
            ValueType = DynamicPropertyValueType.ShortText,
            CreatedBy = "Auto"
        };

        public static DynamicProperty isTrendingHeader = new DynamicProperty
        {
            Id = "isTrending_FrontMatterHeader",
            Name = "is-trending",
            ObjectType = frontMatterHeaderType,
            ValueType = DynamicPropertyValueType.Boolean,
            CreatedBy = "Auto"
        };


        public static DynamicProperty dateHeader = new DynamicProperty
        {
            Id = "date_FrontMatterHeader",
            Name = "date",
            ObjectType = frontMatterHeaderType,
            ValueType = DynamicPropertyValueType.DateTime,
            CreatedBy = "Auto"
        };

        //Create DefaultTheme dynamic property for  Store 
        public static DynamicProperty defaultThemeNameProperty = new DynamicProperty
        {
            Id = "Default_Theme_Name_Property",
            Name = "DefaultThemeName",
            ObjectType = "VirtoCommerce.StoreModule.Core.Model.Store",
            ValueType = DynamicPropertyValueType.ShortText,
            CreatedBy = "Auto"
        };

        //Create Authorize dynamic property for  Store 
        public static DynamicProperty authorizeProperty = new DynamicProperty
        {
            Id = "Authorize_FrontMatterHeader",
            Name = "authorize",
            ObjectType = frontMatterHeaderType,
            ValueType = DynamicPropertyValueType.Boolean,
            CreatedBy = "Auto"
        };

        public static IEnumerable<DynamicProperty> AllDynamicProperties
        {
            get
            {
                {
                    yield return titleHeader;
                    yield return layoutHeader;
                    yield return templateHeader;
                    yield return permalinkHeader;
                    yield return publishedHeader;
                    yield return categoryHeader;
                    yield return categoriesHeader;
                    yield return tagsHeader;
                    yield return aliasesHeader;
                    yield return isStickedHeader;
                    yield return mainImageHeader;
                    yield return isTrendingHeader;
                    yield return dateHeader;
                    yield return defaultThemeNameProperty;
                    yield return authorizeProperty;
                }
            }
        }

        #endregion

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as FrontMatterHeaders;

            result.DynamicProperties = DynamicProperties?.Select(x => x.Clone()).OfType<DynamicObjectProperty>().ToList();

            return result;
        }

        #endregion
    }
}
