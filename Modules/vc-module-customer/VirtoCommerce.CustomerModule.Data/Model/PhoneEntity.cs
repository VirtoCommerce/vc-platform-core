using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerModule.Data.Model
{
    public class PhoneEntity : Entity, ICloneable
    {
        [StringLength(64)]
        public string Number { get; set; }

        [StringLength(64)]
        public string Type { get; set; }

        #region Navigation Properties

        public string MemberId { get; set; }
        public virtual MemberEntity Member { get; set; }

        #endregion

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as PhoneEntity;

            if (Member != null)
            {
                result.Member = Member.Clone() as MemberEntity;
            }

            return result;
        }

        #endregion
    }
}
