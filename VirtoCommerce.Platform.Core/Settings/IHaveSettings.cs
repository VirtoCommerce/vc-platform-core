using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Settings
{
    public interface IHaveSettings : IEntity
    {
        string TypeName { get; }
        ICollection<SettingEntry> Settings { get; set; }
    }
}
