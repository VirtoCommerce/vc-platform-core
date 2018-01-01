using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Security
{
    public class UserSearchCriteria : GenericSearchCriteria
    {    
        public string[] AccountTypes { get; set; }     
        public string MemberId { get; set; }
       //TODO: Update UI pagination to use Skip and Take properties
    }
}
