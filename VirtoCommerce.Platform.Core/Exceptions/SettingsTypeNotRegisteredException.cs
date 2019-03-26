using System;

namespace VirtoCommerce.Platform.Core.Exceptions
{
    public class SettingsTypeNotRegisteredException : Exception
    {
        public SettingsTypeNotRegisteredException(string settingsType)
            : base($"Settings for type: {settingsType} not registered, please register it first")
        {
        }
    }
}
