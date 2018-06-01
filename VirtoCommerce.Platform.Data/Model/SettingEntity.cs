using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.Platform.Data.Model
{
    public class SettingEntity : AuditableEntity
    {
        public SettingEntity()
        {
            SettingValues = new NullCollection<SettingValueEntity>();
        }

        [StringLength(128)]
        public string ObjectType { get; set; }

        [StringLength(128)]
        public string ObjectId { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(1024)]
        public string Description { get; set; }

        public bool IsSystem { get; set; }

        [Required]
        [StringLength(64)]
        public string SettingValueType { get; set; }

        public bool IsEnum { get; set; }
        public bool IsMultiValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is locale dependent. If true, the locale must be specified for the values.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is locale dependent; otherwise, <c>false</c>.
        /// </value>
        public bool IsLocaleDependant { get; set; }

        public virtual ObservableCollection<SettingValueEntity> SettingValues { get; set; }


        public virtual SettingEntry ToModel(SettingEntry settingEntry)
        {
            if (settingEntry == null)
                throw new ArgumentNullException(nameof(settingEntry));

            settingEntry.ObjectType = ObjectType;
            settingEntry.ObjectId = ObjectId;
            settingEntry.Name = Name;
            settingEntry.Description = Description;
            settingEntry.IsArray = IsEnum;
            settingEntry.ValueType = EnumUtility.SafeParse(SettingValueType, Core.Settings.SettingValueType.LongText);
            var stringValues = SettingValues.Select(x => x.ToString(settingEntry.ValueType, CultureInfo.InvariantCulture)).ToArray();
            var rawValues = SettingValues.Select(x => x.GetValue(settingEntry.ValueType)).ToArray();
            if (IsEnum)
            {
                settingEntry.ArrayValues = stringValues;
                settingEntry.RawArrayValues = rawValues;
            }
            else
            {
                if (stringValues.Any())
                {
                    settingEntry.Value = stringValues.First();
                    settingEntry.RawValue = rawValues.First();
                }
            }
            return settingEntry;
        }

        public virtual SettingEntity FromModel(SettingEntry settingEntry)
        {
            if (settingEntry == null)
                throw new ArgumentNullException(nameof(settingEntry));

            ObjectType = settingEntry.ObjectType;
            ObjectId = settingEntry.ObjectId;
            Name = settingEntry.Name;
            Description = settingEntry.Description;
            SettingValueType = settingEntry.ValueType.ToString();
            IsEnum = settingEntry.IsArray;

            var valueEntities = new List<string>();
            if (settingEntry.ArrayValues != null)
            {
                valueEntities.AddRange(settingEntry.ArrayValues);
            }
            else if (settingEntry.Value != null)
            {
                valueEntities.Add(settingEntry.Value);
            }

            SettingValues = new ObservableCollection<SettingValueEntity>(valueEntities.Select(x => AbstractTypeFactory<SettingValueEntity>.TryCreateInstance().SetValue(settingEntry.ValueType, x)));

            return this;
        }

        public virtual void Patch(SettingEntity target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            if (!SettingValues.IsNullCollection())
            {
                var comparer = AnonymousComparer.Create((SettingValueEntity x) => x.ToString(EnumUtility.SafeParse(x.ValueType, Core.Settings.SettingValueType.LongText), CultureInfo.InvariantCulture) ?? string.Empty);
                SettingValues.Patch(target.SettingValues, comparer, (sourceSetting, targetSetting) => { });
            }
        }
    }
}
