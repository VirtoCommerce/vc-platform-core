using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.Platform.Web.Model.Profiles
{
    public class UserProfile : Entity, IHaveSettings
    {
        public virtual ICollection<SettingEntry> Settings { get; set; }
        public virtual string TypeName => GetType().Name;
    }
}
