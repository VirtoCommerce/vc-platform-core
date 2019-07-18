using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerModule.Data.Model
{
    public class MemberGroupEntity : Entity, ICloneable
    {
        [StringLength(64)]
        public string Group { get; set; }

        #region Navigation Properties

        public string MemberId { get; set; }
        public virtual MemberEntity Member { get; set; }

        #endregion

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as MemberGroupEntity;

            if (Member != null)
            {
                result.Member = Member.Clone() as MemberEntity;
            }

            return result;
        }

        #endregion
    }
}
