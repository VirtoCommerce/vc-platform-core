using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CoreModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Data.Model
{
    public class PackageTypeEntity : Entity
    {
        [Required]
        [StringLength(254)]
        public string Name { get; set; }

        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }

        [StringLength(16)]
        public string MeasureUnit { get; set; }

        public virtual PackageType ToModel(PackageType packageType)
        {
            packageType.Id = Id;
            packageType.Name = Name;
            packageType.Length = Length;
            packageType.Width = Width;
            packageType.Height = Height;
            packageType.MeasureUnit = MeasureUnit;         
            return packageType;
        }

        public virtual PackageTypeEntity FromModel(PackageType packageType, PrimaryKeyResolvingMap pkMap)
        {
            pkMap.AddPair(packageType, this);

            Id = packageType.Id;
            Name = packageType.Name;
            Length = packageType.Length;
            Width = packageType.Width;
            Height = packageType.Height;
            MeasureUnit = packageType.MeasureUnit;
            return this;
        }

        public virtual void Patch(PackageTypeEntity target)
        {
            target.Name = Name;
            target.Length = Length;
            target.Width = Width;
            target.Height = Height;
            target.MeasureUnit = MeasureUnit;
        }

    }
}
