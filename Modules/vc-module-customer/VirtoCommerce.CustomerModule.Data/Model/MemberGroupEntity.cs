using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerModule.Data.Model
{
    public class MemberGroupEntity : Entity
    {           
        [StringLength(64)]
        //[Index(IsUnique = false)]
        public string Group { get; set; }

        #region Navigation Properties
        public string MemberId { get; set; }
        public virtual MemberEntity Member { get; set; }
        #endregion
     
    }

}
