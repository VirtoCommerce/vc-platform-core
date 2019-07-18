using System;
using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.CustomerModule.Data.Model
{
    public class MemberDynamicPropertyObjectValueEntity : DynamicPropertyObjectValueEntity, ICloneable
    {
        public virtual MemberEntity Member { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as MemberDynamicPropertyObjectValueEntity;

            if (Member != null)
            {
                result.Member = Member.Clone() as MemberEntity;
            }

            return result;
        }

        #endregion
    }
}
