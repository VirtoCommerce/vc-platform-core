using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerModule.Data.Model
{
    public class ContactEntity : MemberEntity
    {
        public ContactEntity()
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
                contact.FirstName = FirstName;
                contact.MiddleName = MiddleName;
                contact.LastName = LastName;
                contact.BirthDate = BirthDate;
                contact.DefaultLanguage = DefaultLanguage;
                contact.FullName = FullName;
                contact.Salutation = Salutation;
                contact.TimeZone = TimeZone;
                contact.TaxPayerId = TaxpayerId;
                contact.PreferredCommunication = PreferredCommunication;
                contact.PreferredDelivery = PreferredDelivery;
                contact.PhotoUrl = PhotoUrl;
                contact.Organizations = MemberRelations.Select(x => x.Ancestor).OfType<OrganizationEntity>().Select(x => x.Id).ToList();
                contact.Name = contact.FullName;
            }
            return member;
        }

        public override MemberEntity FromModel(Member member, PrimaryKeyResolvingMap pkMap)
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

                if (string.IsNullOrEmpty(Name))
                {
                    Name = contact.FullName;
                }

                if (contact.Organizations != null)
                {
                    MemberRelations = new ObservableCollection<MemberRelationEntity>();
                    foreach (var organization in contact.Organizations)
                    {
                        var memberRelation = new MemberRelationEntity
                        {
                            AncestorId = organization,
                            AncestorSequence = 1,
                            DescendantId = Id,
                        };
                        MemberRelations.Add(memberRelation);
                    }
                }
            }
            //Call base converter
            return base.FromModel(member, pkMap);
        }

        public override void Patch(MemberEntity memberDataEntity)
        {
            var target = memberDataEntity as ContactEntity;

            target.FirstName = FirstName;
            target.MiddleName = MiddleName;
            target.LastName = LastName;
            target.BirthDate = BirthDate;
            target.DefaultLanguage = DefaultLanguage;
            target.FullName = FullName;
            target.Salutation = Salutation;
            target.TimeZone = TimeZone;
            target.TaxpayerId = TaxpayerId;
            target.PreferredCommunication = PreferredCommunication;
            target.PreferredDelivery = PreferredDelivery;
            target.PhotoUrl = PhotoUrl;

            base.Patch(target);
        }
    }
}
