using System.Collections.Generic;

namespace VirtoCommerce.Platform.Core.Settings
{
    public class ObjectSettingEntry : SettingDescriptor
    {
        public ObjectSettingEntry()
        {
        }
        public ObjectSettingEntry(SettingDescriptor descriptor)
        {
            RestartRequired = descriptor.RestartRequired;
            ModuleId = descriptor.ModuleId;
            GroupName = descriptor.GroupName;
            Name = descriptor.Name;
            ValueType = descriptor.ValueType;
            AllowedValues = descriptor.AllowedValues;
            DefaultValue = descriptor.DefaultValue;
            IsDictionary = descriptor.IsDictionary;
        }
        /// <summary>
        /// Setting may belong to any object in system
        /// </summary>
        public string ObjectId { get; set; }
        public string ObjectType { get; set; }

        public object Value { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return ObjectId;
            yield return ObjectType;
        }
    }
}
