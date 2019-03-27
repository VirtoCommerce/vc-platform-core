using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.Model
{
	public class PricelistAssignmentEntity : AuditableEntity
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


		#region Navigation Properties
		public string PricelistId { get; set; }

		public virtual PricelistEntity Pricelist { get; set; }

        #endregion

        public virtual PricelistAssignment ToModel(PricelistAssignment assignment)
        {
            if (assignment == null)
                throw new ArgumentNullException("assignment");

            assignment.Id = this.Id;
            assignment.CatalogId = this.CatalogId;
            assignment.ConditionExpression = this.ConditionExpression;
            assignment.CreatedBy = this.CreatedBy;
            assignment.CreatedDate = this.CreatedDate;
            assignment.Description = this.Description;
            assignment.EndDate = this.EndDate;
            assignment.ModifiedBy = this.ModifiedBy;
            assignment.ModifiedDate = this.ModifiedDate;
            assignment.Name = this.Name;
            assignment.PredicateVisualTreeSerialized = this.PredicateVisualTreeSerialized;
            assignment.PricelistId = this.PricelistId;
            assignment.Priority = this.Priority;
            assignment.StartDate = this.StartDate;

            if (this.Pricelist != null)
            {
                //Need to make lightweight pricelist
                assignment.Pricelist = AbstractTypeFactory<Pricelist>.TryCreateInstance();
                assignment.Pricelist.Id = this.Pricelist.Id;
                assignment.Pricelist.Currency = this.Pricelist.Currency;
                assignment.Pricelist.Description = this.Pricelist.Description;
                assignment.Pricelist.Name = this.Pricelist.Name;

            }

            return assignment;
        }

        public virtual PricelistAssignmentEntity FromModel(PricelistAssignment assignment, PrimaryKeyResolvingMap pkMap)
        {
            if (assignment == null)
                throw new ArgumentNullException("assignment");

            pkMap.AddPair(assignment, this);

            this.Id = assignment.Id;
            this.CatalogId = assignment.CatalogId;
            this.ConditionExpression = assignment.ConditionExpression;
            this.CreatedBy = assignment.CreatedBy;
            this.CreatedDate = assignment.CreatedDate;
            this.Description = assignment.Description;
            this.EndDate = assignment.EndDate;
            this.ModifiedBy = assignment.ModifiedBy;
            this.ModifiedDate = assignment.ModifiedDate;
            this.Name = assignment.Name;
            this.PredicateVisualTreeSerialized = assignment.PredicateVisualTreeSerialized;
            this.PricelistId = assignment.PricelistId;
            this.Priority = assignment.Priority;
            this.StartDate = assignment.StartDate;

            return this;
        }

        public virtual void Patch(PricelistAssignmentEntity target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            target.Name = this.Name;
            target.Description = this.Description;
            target.StartDate = this.StartDate;
            target.EndDate = this.EndDate;
            target.CatalogId = this.CatalogId;
            target.PricelistId = this.PricelistId;
            target.Priority = this.Priority;
            target.ConditionExpression = this.ConditionExpression;
            target.PredicateVisualTreeSerialized = this.PredicateVisualTreeSerialized;        
        }

    }
}
