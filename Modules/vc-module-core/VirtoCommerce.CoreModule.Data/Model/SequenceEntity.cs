using System;
using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.CoreModule.Data.Model
{
    public class SequenceEntity : ICloneable
    {
        [Key]
        [StringLength(256)]
        public string ObjectType { get; set; }

        [Required]
        public int Value { get; set; }

        public DateTime? ModifiedDate { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as SequenceEntity;

            return result;
        }

        #endregion
    }
}
