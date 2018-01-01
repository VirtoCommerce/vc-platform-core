using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Security.Model
{
    public class ApplicationUserEntity : IdentityUser, IAuditable
    {
        public ApplicationUserEntity()
        {
            RoleAssignments = new NullCollection<RoleAssignmentEntity>();
            ApiAccounts = new NullCollection<ApiAccountEntity>();
        }

        #region IAuditable members
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [StringLength(64)]
        public string CreatedBy { get; set; }
        [StringLength(64)]
        public string ModifiedBy { get; set; }
        #endregion

        /// <summary>
        /// Tenant id
        /// </summary>
        [StringLength(128)]
        public string StoreId { get; set; }
        /// <summary>
        /// The member identity associated with security account  
        /// </summary>
        [StringLength(128)]
        public string MemberId { get; set; }

        public bool IsAdministrator { get; set; }

        [StringLength(128)]
        public string UserType { get; set; }
        [StringLength(128)]
        public string AccountState { get; set; }

        public virtual ObservableCollection<RoleAssignmentEntity> RoleAssignments { get; set; }
        public virtual ObservableCollection<ApiAccountEntity> ApiAccounts { get; set; }
    }
}
