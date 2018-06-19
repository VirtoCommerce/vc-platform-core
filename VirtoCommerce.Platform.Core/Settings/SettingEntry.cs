using System;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Settings
{
    /// <summary>
    /// Represent setting record
    /// </summary>
    public class SettingEntry : ValueObject, ICloneable
    {
        /// <summary>
        /// The flag indicates that you need to restart the application to apply this setting changes.
        /// </summary>
        public bool RestartRequired { get; set; }
        public string ModuleId { get; set; }
        /// <summary>
        /// Setting may belong to any object in system
        /// </summary>
        public string ObjectId { get; set; }
        public string ObjectType { get; set; }
        /// <summary>
        /// Setting group name
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// Setting name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Setting string value
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Setting raw value
        /// </summary>
        public object RawValue { get; set; }
        public SettingValueType ValueType { get; set; }
        public string[] AllowedValues { get; set; }
        public string DefaultValue { get; set; }
        public object RawDefaultValue { get; set; }
        public bool IsArray { get; set; }
        public string[] ArrayValues { get; set; }
        public object[] RawArrayValues { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Flag for runtime registered settings 
        /// </summary>
        public bool IsRuntime { get; set; }

        public object Clone()
        {
            return MemberwiseClone() as SettingEntry;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return ObjectId;
            yield return ObjectType;
        }
    }
}
