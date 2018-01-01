using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Platform.Core.Common
{
    public abstract class GenericSearchCriteria : ValueObject
    {
        public string ResponseGroup { get; set; }

        /// <summary>
        /// Search object type
        /// </summary>
        public virtual string ObjectType { get; set; }

        private string[] _objectTypes;
        public virtual string[] ObjectTypes
        {
            get
            {
                if (_objectTypes == null && !string.IsNullOrEmpty(ObjectType))
                {
                    _objectTypes = new[] { ObjectType };
                }
                return _objectTypes;
            }
            set
            {
                _objectTypes = value;
            }
        }

        public IList<string> ObjectIds { get; set; }

        public string Keyword { get; set; }

        /// <summary>
        /// Search phrase language 
        /// </summary>
        public string LanguageCode { get; set; }
     
        public virtual SortInfo[] SortInfos { get; set; }


        public int Skip { get; set; }
        public int Take { get; set; } = 20;
    }
}
