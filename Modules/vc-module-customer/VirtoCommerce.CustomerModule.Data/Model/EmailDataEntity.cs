using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerModule.Data.Model
{

    public class EmailDataEntity : Entity
    {
        [EmailAddress]
        [StringLength(254)]
        public string Address { get; set; }

        public bool IsValidated { get; set; }

        [StringLength(64)]
        public string Type { get; set; }


        #region Navigation Properties

        public string MemberId { get; set; }

        public virtual MemberDataEntity Member { get; set; }


        #endregion

    }


}
