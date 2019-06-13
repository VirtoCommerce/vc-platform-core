using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.Model
{
    public class PricelistEntity : AuditableEntity
    {
        public PricelistEntity()
        {
            Prices = new NullCollection<PriceEntity>();
            Assignments = new NullCollection<PricelistAssignmentEntity>();
        }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        [Required]
        [StringLength(64)]
        public string Currency { get; set; }


        #region Navigation Properties

        public virtual ObservableCollection<PriceEntity> Prices { get; set; }
        public virtual ObservableCollection<PricelistAssignmentEntity> Assignments { get; set; }

        #endregion


        public virtual Pricelist ToModel(Pricelist pricelist)
        {
            if (pricelist == null)
                throw new ArgumentNullException("pricelist");

            pricelist.Id = Id;
            pricelist.CreatedBy = CreatedBy;
            pricelist.CreatedDate = CreatedDate;
            pricelist.ModifiedBy = ModifiedBy;
            pricelist.ModifiedDate = ModifiedDate;
            pricelist.OuterId = OuterId;

            pricelist.Currency = Currency;
            pricelist.Description = Description;
            pricelist.Name = Name;

            pricelist.Assignments = new List<PricelistAssignment>();
            //Create lightweight assignment for represent assignment info in pricelist
            foreach (var assignemntEntity in Assignments)
            {
                var assignment = AbstractTypeFactory<PricelistAssignment>.TryCreateInstance();
                assignment.Id = assignemntEntity.Id;
                assignment.CatalogId = assignemntEntity.CatalogId;
                assignment.Description = assignemntEntity.Description;
                assignment.Name = assignemntEntity.Name;
                assignment.Priority = assignemntEntity.Priority;
                assignment.StartDate = assignemntEntity.StartDate;

                pricelist.Assignments.Add(assignment);
            }
            return pricelist;
        }

        public virtual PricelistEntity FromModel(Pricelist pricelist, PrimaryKeyResolvingMap pkMap)
        {
            if (pricelist == null)
                throw new ArgumentNullException("pricelist");

            pkMap.AddPair(pricelist, this);

            Id = pricelist.Id;
            CreatedBy = pricelist.CreatedBy;
            CreatedDate = pricelist.CreatedDate;
            ModifiedBy = pricelist.ModifiedBy;
            ModifiedDate = pricelist.ModifiedDate;
            OuterId = pricelist.OuterId;

            Currency = pricelist.Currency;
            Description = pricelist.Description;
            Name = pricelist.Name;

            return this;
        }

        public virtual void Patch(PricelistEntity target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            target.Name = Name;
            target.Currency = Currency;
            target.Description = Description;
        }
    }
}
