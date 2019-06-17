using System;
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
            SettingAllowedValues = new NullCollection<SettingValueEntity>();
        }

        [StringLength(128)]
        public string ObjectType { get; set; }

        [StringLength(128)]
        public string ObjectId { get; set; }

        [StringLength(128)]
        public string Name { get; set; }

        public bool IsDictionary { get; set; }

        public bool IsMultiValue { get; set; }

        /// <summary>
        /// Values for current settings
        /// </summary>
        public virtual ObservableCollection<SettingValueEntity> SettingValues { get; set; }

        /// <summary>
        /// Allowed values that settings can have
        /// </summary>
        public virtual ObservableCollection<SettingValueEntity> SettingAllowedValues { get; set; }

        public virtual ObjectSettingEntry ToModel(ObjectSettingEntry objSetting)
        {
            if (objSetting == null)
            {
                throw new ArgumentNullException(nameof(objSetting));
            }

            objSetting.Name = Name;
            objSetting.ObjectType = ObjectType;
            objSetting.ObjectId = ObjectId;
            objSetting.IsDictionary = IsDictionary;
            objSetting.IsMultiValue = IsMultiValue;

            objSetting.Values = SettingValues.Select(x => x.GetValue()).ToArray();
            objSetting.AllowedValues = SettingAllowedValues.Select(x => x.GetValue()).ToArray();

            return objSetting;
        }

        public virtual SettingEntity FromModel(ObjectSettingEntry objectSettingEntry)
        {
            if (objectSettingEntry == null)
            {
                throw new ArgumentNullException(nameof(objectSettingEntry));
            }
            ObjectType = objectSettingEntry.ObjectType;
            ObjectId = objectSettingEntry.ObjectId;
            Name = objectSettingEntry.Name;

            if (objectSettingEntry.IsDictionary)
            {
                SettingValues = new ObservableCollection<SettingValueEntity>(objectSettingEntry.AllowedValues.Select(x => new SettingValueEntity().SetValue(objectSettingEntry.ValueType, x)));
            }
            else
            {
                SettingValues = new ObservableCollection<SettingValueEntity>(new[] { new SettingValueEntity().SetValue(objectSettingEntry.ValueType, objectSettingEntry.Values) });
            }
            return this;
        }

        public virtual void Patch(SettingEntity target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (!SettingValues.IsNullCollection())
            {
                var comparer = AnonymousComparer.Create((SettingValueEntity x) => x.ToString(EnumUtility.SafeParse(x.ValueType, SettingValueType.LongText), CultureInfo.InvariantCulture) ?? string.Empty);
                SettingValues.Patch(target.SettingValues, comparer, (sourceSetting, targetSetting) => { });
            }
        }
    }
}
