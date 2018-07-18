using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerModule.Data.Model
{
	public abstract class MemberDataEntity : AuditableEntity
	{
		public MemberDataEntity()
		{
			Notes = new NullCollection<NoteDataEntity>();
			Addresses = new NullCollection<AddressDataEntity>();
			MemberRelations = new NullCollection<MemberRelationDataEntity>();
			Phones = new NullCollection<PhoneDataEntity>();
			Emails = new NullCollection<EmailDataEntity>();
            Groups = new NullCollection<MemberGroupDataEntity>();
        }

        [StringLength(64)]
        public string MemberType { get; set; }
        
        [StringLength(128)]
        public string Name { get; set; }

        #region NavigationProperties

        public ObservableCollection<NoteDataEntity> Notes { get; set; }

		public ObservableCollection<AddressDataEntity> Addresses { get; set; }

		public ObservableCollection<MemberRelationDataEntity> MemberRelations { get; set; }

		public ObservableCollection<PhoneDataEntity> Phones { get; set; }

		public ObservableCollection<EmailDataEntity> Emails { get; set; }

        public ObservableCollection<MemberGroupDataEntity> Groups { get; set; }

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


        public virtual MemberDataEntity FromModel(Member member, PrimaryKeyResolvingMap pkMap)
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
                this.Phones = new ObservableCollection<PhoneDataEntity>();
                foreach(var phone in member.Phones)
                {
                    var phoneEntity = AbstractTypeFactory<PhoneDataEntity>.TryCreateInstance();
                    phoneEntity.Number = phone;
                    phoneEntity.MemberId = member.Id;
                    this.Phones.Add(phoneEntity);
                }              
            }

            if (member.Groups != null)
            {
                this.Groups = new ObservableCollection<MemberGroupDataEntity>();
                foreach (var group in member.Groups)
                {
                    var groupEntity = AbstractTypeFactory<MemberGroupDataEntity>.TryCreateInstance();
                    groupEntity.Group = group;
                    groupEntity.MemberId = member.Id;
                    this.Groups.Add(groupEntity);
                }
            }

            if (member.Emails != null)
            {
                this.Emails = new ObservableCollection<EmailDataEntity>();
                foreach (var email in member.Emails)
                {
                    var emailEntity = AbstractTypeFactory<EmailDataEntity>.TryCreateInstance();
                    emailEntity.Address = email;
                    emailEntity.MemberId = member.Id;
                    this.Emails.Add(emailEntity);
                }
            }

            if (member.Addresses != null)
            {
                this.Addresses = new ObservableCollection<AddressDataEntity>(member.Addresses.Select(x => AbstractTypeFactory<AddressDataEntity>.TryCreateInstance().FromModel(x)));
                foreach (var address in this.Addresses)
                {
                    address.MemberId = member.Id;
                }
            }

            if (member.Notes != null)
            {
                this.Notes = new ObservableCollection<NoteDataEntity>(member.Notes.Select(x => AbstractTypeFactory<NoteDataEntity>.TryCreateInstance().FromModel(x)));
                foreach (var note in this.Notes)
                {
                    note.MemberId = member.Id;
                }
            }
            return this;
        }

      
        public virtual void Patch(MemberDataEntity target)
        {
            target.Name = Name;
            target.MemberType = MemberType;

            if (!this.Phones.IsNullCollection())
            {
                var phoneComparer = AnonymousComparer.Create((PhoneDataEntity x) => x.Number);
                this.Phones.Patch(target.Phones, phoneComparer, (sourcePhone, targetPhone) => targetPhone.Number = sourcePhone.Number);
            }

            if (!this.Emails.IsNullCollection())
            {
                var addressComparer = AnonymousComparer.Create((EmailDataEntity x) => x.Address);
                this.Emails.Patch(target.Emails, addressComparer, (sourceEmail, targetEmail) => targetEmail.Address = sourceEmail.Address);
            }

            if (!this.Groups.IsNullCollection())
            {
                var groupComparer = AnonymousComparer.Create((MemberGroupDataEntity x) => x.Group);
                this.Groups.Patch(target.Groups, groupComparer, (sourceGroup, targetGroup) => targetGroup.Group = sourceGroup.Group);
            }

            if (!this.Addresses.IsNullCollection())
            {
                this.Addresses.Patch(target.Addresses, (sourceAddress, targetAddress) => sourceAddress.Patch(targetAddress));
            }

            if (!this.Notes.IsNullCollection())
            {
                var noteComparer = AnonymousComparer.Create((NoteDataEntity x) => x.Id ?? x.Body);
                this.Notes.Patch(target.Notes, noteComparer, (sourceNote, targetNote) => sourceNote.Patch(targetNote));
            }

            if (!this.MemberRelations.IsNullCollection())
            {
                var relationComparer = AnonymousComparer.Create((MemberRelationDataEntity x) => x.AncestorId);
                this.MemberRelations.Patch(target.MemberRelations, relationComparer, (sourceRel, targetRel) => { /*Nothing todo*/ });
            }
        }
    }
}
