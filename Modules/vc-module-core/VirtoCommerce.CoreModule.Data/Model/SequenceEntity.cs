using System;
using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.CoreModule.Data.Model
{
    public class SequenceEntity
    {
        [Key]
        [StringLength(256)]
        public string ObjectType { get; set; }

        [Required]
        public int Value { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
