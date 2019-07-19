using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.CustomerModule.Data.Model
{
    public class MemberDynamicPropertyObjectValueEntity : DynamicPropertyObjectValueEntity
    {
        public virtual MemberEntity Member { get; set; }

        #region ICloneable members

        public override object Clone()
        {
            var result = base.Clone() as MemberDynamicPropertyObjectValueEntity;

            if (Member != null)
            {
                result.Member = Member.Clone() as MemberEntity;
            }

            return result;
        }

        #endregion
    }
}
