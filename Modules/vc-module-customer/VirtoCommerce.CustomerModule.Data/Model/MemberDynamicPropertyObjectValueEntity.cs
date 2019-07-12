using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.CustomerModule.Data.Model
{
    public class MemberDynamicPropertyObjectValueEntity : DynamicPropertyObjectValueEntity
    {
        public virtual MemberEntity Member { get; set; }
    }
}
