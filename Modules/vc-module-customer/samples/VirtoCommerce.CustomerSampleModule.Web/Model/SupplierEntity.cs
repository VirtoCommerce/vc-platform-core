using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerSampleModule.Web.Model
{
    public class SupplierEntity : MemberEntity
    {
        [StringLength(128)]
        public string ContractNumber { get; set; }

        public override Member ToModel(Member member)
        {
            //Call base converter first
            base.ToModel(member);

            var member2 = member as Supplier;
            if (member2 != null)
            {
                member2.ContractNumber = ContractNumber;
            }
            return member;
        }

        public override MemberEntity FromModel(Member member, PrimaryKeyResolvingMap pkMap)
        {
            var member2 = member as Supplier;

            if (member2 != null)
            {
                ContractNumber = member2.ContractNumber;
            }

            //Call base converter
            return base.FromModel(member, pkMap);
        }

        public override void Patch(MemberEntity memberEntity)
        {
            var target = memberEntity as SupplierEntity;

            target.ContractNumber = ContractNumber;

            base.Patch(target);
        }
    }
}
