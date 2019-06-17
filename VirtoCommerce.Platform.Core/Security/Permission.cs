using System;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Security
{
    public class Permission : ValueObject
    {
        [Obsolete("Left for backward compatibility")]
        public string Id => Name;

        public string Name { get; set; }
        /// <summary>
        /// Id of the module which has registered this permission.
        /// </summary>
        public string ModuleId { get; set; }
        /// <summary>
        /// Display name of the group to which this permission belongs. The '|' character is used to separate Child and parent groups.
        /// </summary>
        public string GroupName { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return ModuleId;
        }

        public virtual void Patch(Permission target)
        {
            target.Name = Name;
            target.ModuleId = ModuleId;
            target.GroupName = GroupName;
        }
    }
}
