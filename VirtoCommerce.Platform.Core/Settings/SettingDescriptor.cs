using System;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Settings
{
    /// <summary>
    /// Represent setting meta description
    /// </summary>
    public class SettingDescriptor : ValueObject
    {
        /// <summary>
        /// The flag indicates that you need to restart the application to apply this setting changes.
        /// </summary>
        public bool RestartRequired { get; set; }

        /// <summary>
        /// The module id which setting belong to
        /// </summary>
        public string ModuleId { get; set; }

        /// <summary>
        /// Setting group name
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Setting name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of setting value
        /// </summary>
        public SettingValueType ValueType { get; set; }

        /// <summary>
        /// Values that settings can be
        /// </summary>
        public object[] AllowedValues { get; set; }

        /// <summary>
        /// Default settings value (initial settings value)
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// The flag indicates what current setting is just editable dictionary and hasn't any concrete value
        /// </summary>
        public bool IsDictionary { get; set; }

        /// <summary>
        /// The flag indicates that settings may have multiple values
        /// </summary>
        public bool IsMultiValue { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }
    }
}
