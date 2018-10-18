using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerModule.Core.Model.Search
{
    /// <summary>
    /// Represents common member search criteria. More specialized criteria should be derived for this type.
    /// </summary>
    public class MembersSearchCriteria : SearchCriteriaBase
    {
        /// <summary>
        /// Search member type (Contact, Organization etc)
        /// </summary>
        public string MemberType { get; set; }

        private string[] _memberTypes;
        public string[] MemberTypes
        {
            get
            {
                if (_memberTypes == null && !string.IsNullOrEmpty(MemberType))
                {
                    _memberTypes = new[] { MemberType };
                }
                return _memberTypes;
            }
            set
            {
                _memberTypes = value;
            }
        }

        /// <summary>
        /// Search by member Groups  (VIP, Partner etc)
        /// </summary>
        public string Group { get; set; }

        private string[] _groups;
        public string[] Groups
        {
            get
            {
                if (_groups == null && !string.IsNullOrEmpty(Group))
                {
                    _groups = new[] { Group };
                }
                return _groups;
            }
            set
            {
                _groups = value;
            }
        }

        /// <summary>
        /// Search for child members of the given member (members of an organization, for example)
        /// </summary>
        public string MemberId { get; set; }

        /// <summary>
        /// Deep search for child members of the given MemberId or everything if MemberId is empty
        /// </summary>
        public bool DeepSearch { get; set; }

        
    }
}
