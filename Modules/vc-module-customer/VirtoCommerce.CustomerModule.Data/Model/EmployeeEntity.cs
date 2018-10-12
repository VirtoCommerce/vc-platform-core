using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerModule.Data.Model
{
    public class EmployeeEntity : MemberEntity
    {
        [StringLength(64)]
        public string Type { get; set; }

        public bool IsActive { get; set; }

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

        [StringLength(2083)]
        public string PhotoUrl { get; set; }

        public DateTime? BirthDate { get; set; }

        public override Member ToModel(Member member)
        {
            //Call base converter first
            base.ToModel(member);

            var employee = member as Employee;
            if (employee != null)
            {
                employee.FirstName = FirstName;
                employee.MiddleName = MiddleName;
                employee.LastName = LastName;
                employee.BirthDate = BirthDate;
                employee.DefaultLanguage = DefaultLanguage;
                employee.FullName = FullName;
                employee.IsActive = IsActive;
                employee.EmployeeType = Type;
                employee.TimeZone = TimeZone;
                employee.PhotoUrl = PhotoUrl;
                employee.Organizations = MemberRelations.Select(x => x.Ancestor).OfType<OrganizationEntity>().Select(x => x.Id).ToList();
            }
            return member;
        }

        public override MemberEntity FromModel(Member member, PrimaryKeyResolvingMap pkMap)
        {
            var employee = member as Employee;
            if (employee != null)
            {
                member.Name = employee.FullName;
                FirstName = employee.FirstName;
                MiddleName = employee.MiddleName;
                LastName = employee.LastName;
                BirthDate = employee.BirthDate;
                DefaultLanguage = employee.DefaultLanguage;
                FullName = employee.FullName;
                IsActive = employee.IsActive;
                Type = employee.MemberType;
                TimeZone = employee.TimeZone;
                PhotoUrl = employee.PhotoUrl;

                if (employee.Organizations != null)
                {
                    MemberRelations = new ObservableCollection<MemberRelationEntity>();
                    foreach (var organization in employee.Organizations)
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

        public override void Patch(MemberEntity memberEntity)
        {
            if (memberEntity is EmployeeEntity target)
            {
                target.FirstName = this.FirstName;
                target.MiddleName = this.MiddleName;
                target.LastName = this.LastName;
                target.BirthDate = this.BirthDate;
                target.DefaultLanguage = this.DefaultLanguage;
                target.FullName = this.FullName;
                target.IsActive = this.IsActive;
                target.Type = this.Type;
                target.TimeZone = this.TimeZone;
                target.PhotoUrl = this.PhotoUrl;
            }

            base.Patch(memberEntity);
        }
    }
}
