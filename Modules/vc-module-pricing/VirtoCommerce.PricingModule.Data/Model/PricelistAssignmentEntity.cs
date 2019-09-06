using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model.Conditions;

namespace VirtoCommerce.PricingModule.Data.Model
{
    public class PricelistAssignmentEntity : AuditableEntity, IHasOuterId
    {
        [StringLength(128)]
        [Required]
        public string Name { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        public int Priority { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string ConditionExpression { get; set; }

        public string PredicateVisualTreeSerialized { get; set; }

        [StringLength(128)]
        [Required]
        public string CatalogId { get; set; }

        [StringLength(128)]
        public string OuterId { get; set; }

        #region Navigation Properties

        public string PricelistId { get; set; }
        public virtual PricelistEntity Pricelist { get; set; }

        #endregion

        public virtual PricelistAssignment ToModel(PricelistAssignment assignment)
        {
            if (assignment == null)
                throw new ArgumentNullException(nameof(assignment));

            assignment.Id = Id;
            assignment.CreatedBy = CreatedBy;
            assignment.CreatedDate = CreatedDate;
            assignment.ModifiedBy = ModifiedBy;
            assignment.ModifiedDate = ModifiedDate;
            assignment.OuterId = OuterId;

            assignment.CatalogId = CatalogId;
            assignment.Description = Description;
            assignment.EndDate = EndDate;
            assignment.Name = Name;
            assignment.PricelistId = PricelistId;
            assignment.Priority = Priority;
            assignment.StartDate = StartDate;

            if (Pricelist != null)
            {
                //Need to make lightweight pricelist
                assignment.Pricelist = AbstractTypeFactory<Pricelist>.TryCreateInstance();
                assignment.Pricelist.Id = Pricelist.Id;
                assignment.Pricelist.Currency = Pricelist.Currency;
                assignment.Pricelist.Description = Pricelist.Description;
                assignment.Pricelist.Name = Pricelist.Name;

            }
            assignment.DynamicExpression = AbstractTypeFactory<PriceConditionTree>.TryCreateInstance();
            if (PredicateVisualTreeSerialized != null)
            {
                assignment.DynamicExpression = JsonConvert.DeserializeObject<PriceConditionTree>(PredicateVisualTreeSerialized, new ConditionJsonConverter());
            }
            return assignment;
        }

        public virtual PricelistAssignmentEntity FromModel(PricelistAssignment assignment, PrimaryKeyResolvingMap pkMap)
        {
            if (assignment == null)
                throw new ArgumentNullException(nameof(assignment));

            pkMap.AddPair(assignment, this);

            Id = assignment.Id;
            CreatedBy = assignment.CreatedBy;
            CreatedDate = assignment.CreatedDate;
            ModifiedBy = assignment.ModifiedBy;
            ModifiedDate = assignment.ModifiedDate;
            OuterId = assignment.OuterId;

            CatalogId = assignment.CatalogId;
            Description = assignment.Description;
            EndDate = assignment.EndDate;
            Name = assignment.Name;
            PricelistId = assignment.PricelistId;
            Priority = assignment.Priority;
            StartDate = assignment.StartDate;

            if (assignment.DynamicExpression != null)
            {
                PredicateVisualTreeSerialized = JsonConvert.SerializeObject(assignment.DynamicExpression, new ConditionJsonConverter(doNotSerializeAvailCondition: true));
            }

            return this;
        }

        public virtual void Patch(PricelistAssignmentEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.Name = Name;
            target.Description = Description;
            target.StartDate = StartDate;
            target.EndDate = EndDate;
            target.CatalogId = CatalogId;
            target.PricelistId = PricelistId;
            target.Priority = Priority;
            target.PredicateVisualTreeSerialized = PredicateVisualTreeSerialized;
        }
    }
}
