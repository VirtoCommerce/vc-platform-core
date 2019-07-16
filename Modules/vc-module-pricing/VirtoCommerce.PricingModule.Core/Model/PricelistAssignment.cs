using System;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model.Conditions;

namespace VirtoCommerce.PricingModule.Core.Model
{
    /// <summary>
    /// Used to assign pricelist to specific catalog by using conditional expression 
    /// </summary>
	public class PricelistAssignment : AuditableEntity, ICloneable
    {
        public string CatalogId { get; set; }
        public string PricelistId { get; set; }
        public Pricelist Pricelist { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// If two PricelistAssignments satisfies the conditions and rules, will use one with the greater priority
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// Start of period when Prices Assignment is valid. Null value means no limit
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// End of period when Prices Assignment is valid. Null value means no limit
        /// </summary>
		public DateTime? EndDate { get; set; }
        /// <summary>
        /// Serialized condition expression visual tree used in UI
        /// </summary>
		public string PredicateVisualTreeSerialized { get; set; }
        /// <summary>
        /// Deserialized conditional expression  used to evaluate current assignment availability 
        /// </summary>
        // TECHDEBT: [JsonIgnore] attribute here is a workaround to exclude this property from Swagger documentation.
        //           This property causes NSwag to include lots of types including MethodImplAttributes, which leads to the invalid Swagger JSON.
        private Condition[] _conditions;
        [JsonIgnore]
        public Condition[] Conditions => _conditions ?? (_conditions = ((PriceConditionTree)DynamicExpression).GetConditions());

        /// <summary>
        /// List of conditions and rules to define Prices Assignment is valid
        /// </summary>
        public IConditionTree DynamicExpression { get; set; }

        public string OuterId { get; set; }


        #region ICloneable members
        public virtual object Clone()
        {
            var result = MemberwiseClone() as PricelistAssignment;
            if (Pricelist != null)
            {
                result.Pricelist = result.Pricelist.Clone() as Pricelist;
            }
            //TODO: Clone Conditions, DynamicExpression
            return result;
        }
        #endregion
    }
}
