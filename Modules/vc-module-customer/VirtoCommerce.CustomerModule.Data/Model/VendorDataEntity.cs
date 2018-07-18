using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerModule.Data.Model
{
    public class VendorDataEntity : MemberDataEntity
    {
        [StringLength(256)]
        public string Description { get; set; }
        [StringLength(2048)]
        public string SiteUrl { get; set; }
        [StringLength(2048)]
        public string LogoUrl { get; set; }
        [StringLength(64)]
        public string GroupName { get; set; }

        public override Member ToModel(Member member)
        {
            //Call base converter first
            base.ToModel(member);

            if (member is Vendor vendor)
            {
                vendor.SiteUrl = this.SiteUrl;
                vendor.LogoUrl = this.LogoUrl;
                vendor.GroupName = this.GroupName;
                vendor.Description = this.Description;
            }
            
            return member;
        }

        public override MemberDataEntity FromModel(Member member, PrimaryKeyResolvingMap pkMap)
        {
            if (member is Vendor vendor)
            {
                SiteUrl = vendor.SiteUrl;
                LogoUrl = vendor.LogoUrl;
                GroupName = vendor.GroupName;
                Description = vendor.Description;
            }
            //Call base converter
            return base.FromModel(member, pkMap);
        }

        public override void Patch(MemberDataEntity memberEntity)
        {
            var target = memberEntity as VendorDataEntity;

            target.SiteUrl = this.SiteUrl;
            target.LogoUrl = this.LogoUrl;
            target.GroupName = this.GroupName;
            target.Description = this.Description;
                   
            base.Patch(target);
        }
    }
}
