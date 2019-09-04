using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.CustomerModule.Core.Model
{
    public abstract class Member : AuditableEntity, IHasDynamicProperties, ISeoSupport, IHasOuterId, ICloneable
    {
        protected Member()
        {
            MemberType = GetType().Name;
        }

        public string Name { get; set; }
        public string MemberType { get; set; }
        public string OuterId { get; set; }

        public IList<Address> Addresses { get; set; }
        public IList<string> Phones { get; set; }
        public IList<string> Emails { get; set; }
        public IList<Note> Notes { get; set; }
        public IList<string> Groups { get; set; }

        #region IHasDynamicProperties Members

        public virtual string ObjectType => typeof(Member).FullName;
        public ICollection<DynamicObjectProperty> DynamicProperties { get; set; }

        #endregion

        #region ISeoSupport Members

        public virtual string SeoObjectType => GetType().Name;

        public virtual IList<SeoInfo> SeoInfos { get; set; }

        #endregion

        public virtual void ReduceDetails(string responseGroup)
        {
            //Reduce details according to response group
            var memberResponseGroup = EnumUtility.SafeParseFlags(responseGroup, MemberResponseGroup.Full);

            if (!memberResponseGroup.HasFlag(MemberResponseGroup.WithNotes))
            {
                Notes = null;
            }
            if (!memberResponseGroup.HasFlag(MemberResponseGroup.WithAddresses))
            {
                Addresses = null;
            }
            if (!memberResponseGroup.HasFlag(MemberResponseGroup.WithEmails))
            {
                Emails = null;
            }
            if (!memberResponseGroup.HasFlag(MemberResponseGroup.WithGroups))
            {
                Groups = null;
            }
            if (!memberResponseGroup.HasFlag(MemberResponseGroup.WithPhones))
            {
                Phones = null;
            }
            if (!memberResponseGroup.HasFlag(MemberResponseGroup.WithSeo))
            {
                SeoInfos = null;
            }
            if (!memberResponseGroup.HasFlag(MemberResponseGroup.WithDynamicProperties))
            {
                DynamicProperties = null;
            }
        }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as Member;

            if (Notes != null)
            {
                result.Notes = new ObservableCollection<Note>(Notes.Select(x => x.Clone() as Note));
            }

            if (Addresses != null)
            {
                result.Addresses = new ObservableCollection<Address>(Addresses.Select(x => x.Clone() as Address));
            }

            if (SeoInfos != null)
            {
                result.SeoInfos = new ObservableCollection<SeoInfo>(SeoInfos.Select(x => x.Clone() as SeoInfo));
            }

            if (DynamicProperties != null)
            {
                result.DynamicProperties = new ObservableCollection<DynamicObjectProperty>(
                    DynamicProperties.Select(x => x.Clone() as DynamicObjectProperty));
            }

            return result;
        }

        #endregion
    }
}
