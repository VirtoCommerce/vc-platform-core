using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerModule.Data.Model
{
    public class ContactDataEntity : MemberDataEntity
    {
        public ContactDataEntity()
        {
            BirthDate = DateTime.Now;
        }

        #region UserProfile members
        [StringLength(128)]
        public string FirstName { get; set; }

        [StringLength(128)]
        public string MiddleName { get; set; }

        [StringLength(128)]
        public string LastName { get; set; }

        [StringLength(254)]
        [Required]
        public string FullName { get; set; }

        [StringLength(32)]
        public string TimeZone { get; set; }

        [StringLength(32)]
        public string DefaultLanguage { get; set; }

        public DateTime? BirthDate { get; set; }

        [StringLength(64)]
        public string TaxpayerId { get; set; }

        [StringLength(64)]
        public string PreferredDelivery { get; set; }

        [StringLength(64)]
        public string PreferredCommunication { get; set; }

        [StringLength(2083)]
        public string PhotoUrl { get; set; }

        [StringLength(256)]
        public string Salutation { get; set; }

        #endregion

        public override Member ToModel(Member member)
        {
            //Call base converter first
            base.ToModel(member);
            var contact = member as Contact;
            if (contact != null)
            {
                contact.FirstName = this.FirstName;
                contact.MiddleName = this.MiddleName;
                contact.LastName = this.LastName;
                contact.BirthDate = this.BirthDate;
                contact.DefaultLanguage = this.DefaultLanguage;
                contact.FullName = this.FullName;
                contact.Salutation = this.Salutation;
                contact.TimeZone = this.TimeZone;
                contact.TaxPayerId = this.TaxpayerId;
                contact.PreferredCommunication = this.PreferredCommunication;
                contact.PreferredDelivery = this.PreferredDelivery;
                contact.PhotoUrl = this.PhotoUrl;
                contact.Organizations = this.MemberRelations.Select(x => x.Ancestor).OfType<OrganizationDataEntity>().Select(x => x.Id).ToList();
                contact.Name = contact.FullName;
            }
            return member;
        }

        public override MemberDataEntity FromModel(Member member, PrimaryKeyResolvingMap pkMap)
        {
            var contact = member as Contact;
            if (contact != null)
            {
                FirstName = contact.FirstName;
                MiddleName = contact.MiddleName;
                LastName = contact.LastName;
                BirthDate = contact.BirthDate;
                DefaultLanguage = contact.DefaultLanguage;
                FullName = contact.FullName;
                Salutation = contact.Salutation;
                TimeZone = contact.TimeZone;
                TaxpayerId = contact.TaxPayerId;
                PreferredCommunication = contact.PreferredCommunication;
                PreferredDelivery = contact.PreferredDelivery;
                PhotoUrl = contact.PhotoUrl;

                if (string.IsNullOrEmpty(this.Name))
                {
                    this.Name = contact.FullName;
                }

                if (contact.Organizations != null)
                {
                    this.MemberRelations = new ObservableCollection<MemberRelationDataEntity>();
                    foreach (var organization in contact.Organizations)
                    {
                        var memberRelation = new MemberRelationDataEntity
                        {
                            AncestorId = organization,
                            AncestorSequence = 1,
                            DescendantId = this.Id,
                        };
                        this.MemberRelations.Add(memberRelation);
                    }
                }
            }
            //Call base converter
            return base.FromModel(member, pkMap);
        }

        public override void Patch(MemberDataEntity memberDataEntity)
        {
            var target = memberDataEntity as ContactDataEntity;

            target.FirstName = this.FirstName;
            target.MiddleName = this.MiddleName;
            target.LastName = this.LastName;
            target.BirthDate = this.BirthDate;
            target.DefaultLanguage = this.DefaultLanguage;
            target.FullName = this.FullName;
            target.Salutation = this.Salutation;
            target.TimeZone = this.TimeZone;
            target.TaxpayerId = this.TaxpayerId;
            target.PreferredCommunication = this.PreferredCommunication;
            target.PreferredDelivery = this.PreferredDelivery;
            target.PhotoUrl = this.PhotoUrl;

            base.Patch(target);
        }
    }
}
