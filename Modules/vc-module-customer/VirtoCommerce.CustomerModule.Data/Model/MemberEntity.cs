using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerModule.Data.Model
{
	public abstract class MemberEntity : AuditableEntity
	{
		public MemberEntity()
		{
			Notes = new NullCollection<NoteEntity>();
			Addresses = new NullCollection<AddressEntity>();
			MemberRelations = new NullCollection<MemberRelationEntity>();
			Phones = new NullCollection<PhoneEntity>();
			Emails = new NullCollection<EmailEntity>();
            Groups = new NullCollection<MemberGroupEntity>();
        }

        [StringLength(64)]
        public string MemberType { get; set; }
        
        [StringLength(128)]
        public string Name { get; set; }

        #region NavigationProperties

        public ObservableCollection<NoteEntity> Notes { get; set; }

		public ObservableCollection<AddressEntity> Addresses { get; set; }

		public ObservableCollection<MemberRelationEntity> MemberRelations { get; set; }

		public ObservableCollection<PhoneEntity> Phones { get; set; }

		public ObservableCollection<EmailEntity> Emails { get; set; }

        public ObservableCollection<MemberGroupEntity> Groups { get; set; }

        #endregion

        public virtual Member ToModel(Member member)
        {
            if (member == null) throw new ArgumentNullException("member");

            member.Id = Id;
            member.CreatedBy = CreatedBy;
            member.CreatedDate = CreatedDate;
            member.ModifiedBy = ModifiedBy;
            member.ModifiedDate = ModifiedDate;
            member.MemberType = MemberType;
            member.Name = Name;

            member.Addresses = this.Addresses.OrderBy(x => x.Id).Select(x => x.ToModel(AbstractTypeFactory<Address>.TryCreateInstance())).ToList();
            member.Emails = this.Emails.OrderBy(x => x.Id).Select(x => x.Address).ToList();
            member.Notes = this.Notes.OrderBy(x => x.Id).Select(x => x.ToModel(new Note())).ToList();
            member.Phones = this.Phones.OrderBy(x => x.Id).Select(x => x.Number).ToList();
            member.Groups = this.Groups.OrderBy(x => x.Id).Select(x => x.Group).ToList();

            return member;
        }


        public virtual MemberEntity FromModel(Member member, PrimaryKeyResolvingMap pkMap)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            pkMap.AddPair(member, this);

            Id = member.Id;
            CreatedBy = member.CreatedBy;
            CreatedDate = member.CreatedDate;
            ModifiedBy = member.ModifiedBy;
            ModifiedDate = member.ModifiedDate;
            MemberType = member.MemberType;
            Name = member.Name;

            if (member.Phones != null)
            {
                this.Phones = new ObservableCollection<PhoneEntity>();
                foreach(var phone in member.Phones)
                {
                    var phoneEntity = AbstractTypeFactory<PhoneEntity>.TryCreateInstance();
                    phoneEntity.Number = phone;
                    phoneEntity.MemberId = member.Id;
                    this.Phones.Add(phoneEntity);
                }              
            }

            if (member.Groups != null)
            {
                this.Groups = new ObservableCollection<MemberGroupEntity>();
                foreach (var group in member.Groups)
                {
                    var groupEntity = AbstractTypeFactory<MemberGroupEntity>.TryCreateInstance();
                    groupEntity.Group = group;
                    groupEntity.MemberId = member.Id;
                    this.Groups.Add(groupEntity);
                }
            }

            if (member.Emails != null)
            {
                this.Emails = new ObservableCollection<EmailEntity>();
                foreach (var email in member.Emails)
                {
                    var emailEntity = AbstractTypeFactory<EmailEntity>.TryCreateInstance();
                    emailEntity.Address = email;
                    emailEntity.MemberId = member.Id;
                    this.Emails.Add(emailEntity);
                }
            }

            if (member.Addresses != null)
            {
                this.Addresses = new ObservableCollection<AddressEntity>(member.Addresses.Select(x => AbstractTypeFactory<AddressEntity>.TryCreateInstance().FromModel(x)));
                foreach (var address in this.Addresses)
                {
                    address.MemberId = member.Id;
                }
            }

            if (member.Notes != null)
            {
                this.Notes = new ObservableCollection<NoteEntity>(member.Notes.Select(x => AbstractTypeFactory<NoteEntity>.TryCreateInstance().FromModel(x)));
                foreach (var note in this.Notes)
                {
                    note.MemberId = member.Id;
                }
            }
            return this;
        }

      
        public virtual void Patch(MemberEntity target)
        {
            target.Name = Name;
            target.MemberType = MemberType;

            if (!this.Phones.IsNullCollection())
            {
                var phoneComparer = AnonymousComparer.Create((PhoneEntity x) => x.Number);
                this.Phones.Patch(target.Phones, phoneComparer, (sourcePhone, targetPhone) => targetPhone.Number = sourcePhone.Number);
            }

            if (!this.Emails.IsNullCollection())
            {
                var addressComparer = AnonymousComparer.Create((EmailEntity x) => x.Address);
                this.Emails.Patch(target.Emails, addressComparer, (sourceEmail, targetEmail) => targetEmail.Address = sourceEmail.Address);
            }

            if (!this.Groups.IsNullCollection())
            {
                var groupComparer = AnonymousComparer.Create((MemberGroupEntity x) => x.Group);
                this.Groups.Patch(target.Groups, groupComparer, (sourceGroup, targetGroup) => targetGroup.Group = sourceGroup.Group);
            }

            if (!this.Addresses.IsNullCollection())
            {
                this.Addresses.Patch(target.Addresses, (sourceAddress, targetAddress) => sourceAddress.Patch(targetAddress));
            }

            if (!this.Notes.IsNullCollection())
            {
                var noteComparer = AnonymousComparer.Create((NoteEntity x) => x.Id ?? x.Body);
                this.Notes.Patch(target.Notes, noteComparer, (sourceNote, targetNote) => sourceNote.Patch(targetNote));
            }

            if (!this.MemberRelations.IsNullCollection())
            {
                var relationComparer = AnonymousComparer.Create((MemberRelationEntity x) => x.AncestorId);
                this.MemberRelations.Patch(target.MemberRelations, relationComparer, (sourceRel, targetRel) => { /*Nothing todo*/ });
            }
        }
    }
}
