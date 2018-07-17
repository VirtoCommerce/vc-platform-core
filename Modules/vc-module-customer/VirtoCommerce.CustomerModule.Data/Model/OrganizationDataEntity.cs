using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using System.Collections.ObjectModel;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.CustomerModule.Data.Model
{
    public class OrganizationDataEntity : MemberDataEntity
	{
 		[StringLength(256)]
		public string Description { get; set; }

 		[StringLength(64)]
		public string BusinessCategory { get; set; }

  		[StringLength(128)]
		public string OwnerId { get; set; }

        public override Member ToModel(Member member)
        {
            //Call base converter first
            base.ToModel(member);

            var organization = member as Organization;
            if (organization != null)
            {
                organization.Description = this.Description;
                organization.OwnerId = this.OwnerId;
                organization.BusinessCategory = this.BusinessCategory;
                if (MemberRelations.Any())
                {
                    organization.ParentId = MemberRelations.FirstOrDefault().AncestorId;
                }
                
            }
            return member;
        }

        public override MemberDataEntity FromModel(Member member, PrimaryKeyResolvingMap pkMap)
        {
            var organization = member as Organization;
         
            if (organization != null && organization.ParentId != null)
            {
                Description = organization.Description;
                OwnerId = organization.OwnerId;
                BusinessCategory = organization.BusinessCategory;

                this.MemberRelations = new ObservableCollection<MemberRelationDataEntity>();
                var memberRelation = new MemberRelationDataEntity
                {
                    AncestorId = organization.ParentId,
                    DescendantId = organization.Id,
                    AncestorSequence = 1
                };
                this.MemberRelations.Add(memberRelation);
            }

            //Call base converter
            return base.FromModel(member, pkMap);
        }

        public override void Patch(MemberDataEntity memberEntity)
        {
            var target = memberEntity as OrganizationDataEntity;

            target.Name = this.Name;
            target.Description = this.Description;
            target.OwnerId = this.OwnerId;
            target.BusinessCategory = this.BusinessCategory;

            base.Patch(target);
        }
    }
}
