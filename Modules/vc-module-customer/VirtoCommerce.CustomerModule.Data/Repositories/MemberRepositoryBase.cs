using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CustomerModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CustomerModule.Data.Repositories
{
    public abstract class MemberRepositoryBase : DbContextRepositoryBase<CustomerDbContext>, IMemberRepository
    {
        private readonly MethodInfo _genericGetMembersMethodInfo;

        public MemberRepositoryBase(CustomerDbContext dbContext) : base(dbContext)
        {
            _genericGetMembersMethodInfo = typeof(MemberRepositoryBase).GetMethod("InnerGetMembersByIds");
        }

        #region IMemberRepository Members


        public IQueryable<AddressDataEntity> Addresses => DbContext.Set<AddressDataEntity>(); 
        public IQueryable<EmailDataEntity> Emails => DbContext.Set<EmailDataEntity>();
        public IQueryable<MemberGroupDataEntity> Groups => DbContext.Set<MemberGroupDataEntity>();
        public IQueryable<NoteDataEntity> Notes => DbContext.Set<NoteDataEntity>(); 
        public IQueryable<PhoneDataEntity> Phones => DbContext.Set<PhoneDataEntity>();
        public IQueryable<MemberDataEntity> Members => DbContext.Set<MemberDataEntity>(); 
        public IQueryable<MemberRelationDataEntity> MemberRelations => DbContext.Set<MemberRelationDataEntity>(); 

        public virtual async Task<MemberDataEntity[]> GetMembersByIdsAsync(string[] ids,  string responseGroup = null, string[] memberTypes = null)
        {         
            if(ids.IsNullOrEmpty())
            {
                return new MemberDataEntity[] { };
            }

            var result = new List<MemberDataEntity>();         
            if (!memberTypes.IsNullOrEmpty())
            {
                foreach (var memberType in memberTypes)
                {
                    //Use special dynamically constructed inner generic method for each passed member type 
                    //for better query performance
                    var gm = _genericGetMembersMethodInfo.MakeGenericMethod(Type.GetType(memberType));
                    var members = gm.Invoke(this, new object[] { ids, responseGroup }) as MemberDataEntity[];
                    result.AddRange(members);
                    //Stop process other types
                    if (result.Count() == ids.Count())
                    {
                        break;
                    }
                }
            }         
            else
            {
                var members = await InnerGetMembersByIds<MemberDataEntity>(ids, responseGroup);
                result.AddRange(members);
            }
            return result.ToArray();
        }

        public virtual async Task RemoveMembersByIdsAsync(string[] ids, string[] memberTypes = null)
        {
            var dbMembers = await GetMembersByIdsAsync(ids, null, memberTypes);
            foreach (var dbMember in dbMembers)
            {
                foreach (var relation in dbMember.MemberRelations.ToArray())
                {
                    Remove(relation);
                }
                Remove(dbMember);
            }
        }
        #endregion

        public async Task<T[]> InnerGetMembersByIds<T>(string[] ids, string responseGroup = null) where T: MemberDataEntity
        {
            //Use OfType() clause very much accelerates the query performance when used TPT inheritance
            var query = Members.OfType<T>().Where(x => ids.Contains(x.Id));
           
            var retVal = query.ToArray();
            ids = retVal.Select(x => x.Id).ToArray();
            if (!ids.IsNullOrEmpty())
            {
                var relations = await MemberRelations.Where(x => ids.Contains(x.DescendantId)).ToArrayAsync();
                var ancestorIds = relations.Select(x => x.AncestorId).ToArray();
                if (!ancestorIds.IsNullOrEmpty())
                {
                    var ancestors = await Members.Where(x => ancestorIds.Contains(x.Id)).ToArrayAsync();
                }
                var notes = Notes.Where(x => ids.Contains(x.MemberId)).ToArrayAsync();
                var emails = Emails.Where(x => ids.Contains(x.MemberId)).ToArrayAsync();
                var addresses = Addresses.Where(x => ids.Contains(x.MemberId)).ToArrayAsync();
                var phones = Phones.Where(x => ids.Contains(x.MemberId)).ToArrayAsync();
                var groups = Groups.Where(x => ids.Contains(x.MemberId)).ToArrayAsync();

                await Task.WhenAll(notes, emails, addresses, phones, groups);
            }

            return retVal;
        }

    }
}
