using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerSampleModule.Web.Model
{
    public class Contact2Entity : ContactEntity
    {
        [StringLength(128)]
        public string JobTitle { get; set; }

        public override MemberEntity FromModel(Member member, PrimaryKeyResolvingMap pkMap)
        {
            var contact2 = member as Contact2;
            if (contact2 != null)
            {
                JobTitle = contact2.JobTitle;
            }
            //Call base converter
            return base.FromModel(member, pkMap);
        }

        public override Member ToModel(Member member)
        {
            //Call base converter first
            base.ToModel(member);
            var contact2 = member as Contact2;
            if (contact2 != null)
            {
                contact2.JobTitle = JobTitle;
            }
            return member;
        }

        public override void Patch(MemberEntity memberDataEntity)
        {
            var target = memberDataEntity as Contact2Entity;
            target.JobTitle = JobTitle;
            base.Patch(target);
        }
    }
}
