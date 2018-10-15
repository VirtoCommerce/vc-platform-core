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

            pricelist.Id = this.Id;
            pricelist.CreatedBy = this.CreatedBy;
            pricelist.CreatedDate = this.CreatedDate;
            pricelist.Currency = this.Currency;
            pricelist.Description = this.Description;
            pricelist.ModifiedBy = this.ModifiedBy;
            pricelist.ModifiedDate = this.ModifiedDate;
            pricelist.Name = this.Name;

            pricelist.Assignments = new List<PricelistAssignment>();
            //Create lightweight assignment for represent assignment info in pricelist
            foreach(var assignemntEntity in this.Assignments)
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

            this.Id = pricelist.Id;
            this.CreatedBy = pricelist.CreatedBy;
            this.CreatedDate = pricelist.CreatedDate;
            this.Currency = pricelist.Currency;
            this.Description = pricelist.Description;
            this.ModifiedBy = pricelist.ModifiedBy;
            this.ModifiedDate = pricelist.ModifiedDate;
            this.Name = pricelist.Name;
        
            return this;
        }

        public virtual void Patch(PricelistEntity target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            target.Name = this.Name;
            target.Currency = this.Currency;
            target.Description = this.Description;

        }
    }
}
