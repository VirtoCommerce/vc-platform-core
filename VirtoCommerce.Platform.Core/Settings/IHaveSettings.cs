using System.Collections.Generic;

namespace VirtoCommerce.Platform.Core.Settings
{
    public interface IHaveSettings
	{
		ICollection<SettingEntry> Settings { get; set; }
	}
}
