using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Promotions;
using VirtoCommerce.MarketingModule.Data.Promotions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Model
{
    public class PromotionEntity : AuditableEntity, IHasOuterId
    {
        [StringLength(128)]
        public string StoreId { get; set; }

        [StringLength(128)]
        public string CatalogId { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(1024)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int Priority { get; set; }

        public bool IsExclusive { get; set; }

        public bool IsAllowCombiningWithSelf { get; set; }

        [NotMapped]
        public bool HasCoupons { get; set; }

        public string PredicateSerialized { get; set; }

        public string PredicateVisualTreeSerialized { get; set; }

        public string RewardsSerialized { get; set; }

        public int PerCustomerLimit { get; set; }

        public int TotalLimit { get; set; }

        [StringLength(128)]
        public string OuterId { get; set; }

        #region Navigation Properties

        public virtual ObservableCollection<PromotionStoreEntity> Stores { get; set; } = new NullCollection<PromotionStoreEntity>();

        #endregion

        public virtual Promotion ToModel(Promotion promotion)
        {
            if (promotion == null)
            {
                throw new ArgumentNullException(nameof(promotion));
            }

            promotion.Id = Id;
            promotion.CreatedBy = CreatedBy;
            promotion.CreatedDate = CreatedDate;
            promotion.ModifiedBy = ModifiedBy;
            promotion.ModifiedDate = ModifiedDate;
            promotion.OuterId = OuterId;

            promotion.StartDate = StartDate;
            promotion.EndDate = EndDate;
            promotion.Store = StoreId;
            promotion.Name = Name;
            promotion.Description = Description;
            promotion.IsActive = IsActive;
            promotion.EndDate = EndDate;
            promotion.Priority = Priority;
            promotion.IsExclusive = IsExclusive;
            promotion.MaxPersonalUsageCount = PerCustomerLimit;
            promotion.MaxUsageCount = TotalLimit;
            promotion.MaxPersonalUsageCount = PerCustomerLimit;
            promotion.HasCoupons = HasCoupons;

            if (Stores != null)
            {
                promotion.StoreIds = Stores.Select(x => x.StoreId).ToList();
            }
            if (promotion is DynamicPromotion dynamicPromotion)
            {
                dynamicPromotion.IsAllowCombiningWithSelf = IsAllowCombiningWithSelf;
                dynamicPromotion.DynamicExpression = AbstractTypeFactory<PromotionConditionAndRewardTree>.TryCreateInstance();
                if (PredicateVisualTreeSerialized != null)
                {
                    dynamicPromotion.DynamicExpression = JsonConvert.DeserializeObject<PromotionConditionAndRewardTree>(PredicateVisualTreeSerialized, new ConditionJsonConverter(), new RewardJsonConverter());
                }
            }
            return promotion;
        }

        public virtual PromotionEntity FromModel(Promotion promotion, PrimaryKeyResolvingMap pkMap)
        {
            if (promotion == null)
            {
                throw new ArgumentNullException(nameof(promotion));
            }

            pkMap.AddPair(promotion, this);

            Id = promotion.Id;
            CreatedBy = promotion.CreatedBy;
            CreatedDate = promotion.CreatedDate;
            ModifiedBy = promotion.ModifiedBy;
            ModifiedDate = promotion.ModifiedDate;
            OuterId = promotion.OuterId;

            StartDate = promotion.StartDate ?? DateTime.UtcNow;
            EndDate = promotion.EndDate;
            StoreId = promotion.Store;
            Name = promotion.Name;
            Description = promotion.Description;
            IsActive = promotion.IsActive;
            EndDate = promotion.EndDate;
            Priority = promotion.Priority;
            IsExclusive = promotion.IsExclusive;
        
            PerCustomerLimit = promotion.MaxPersonalUsageCount;
            TotalLimit = promotion.MaxUsageCount;
            PerCustomerLimit = promotion.MaxPersonalUsageCount;

            if (promotion.StoreIds != null)
            {
                Stores = new ObservableCollection<PromotionStoreEntity>(promotion.StoreIds.Select(x => new PromotionStoreEntity { StoreId = x, PromotionId = promotion.Id }));
            }

            if (promotion is DynamicPromotion dynamicPromotion)
            {
                IsAllowCombiningWithSelf = dynamicPromotion.IsAllowCombiningWithSelf;

                if (dynamicPromotion.DynamicExpression != null)
                {
                    PredicateVisualTreeSerialized = JsonConvert.SerializeObject(dynamicPromotion.DynamicExpression, new ConditionJsonConverter(doNotSerializeAvailCondition: true));
                }
            }
            return this;
        }

        public virtual void Patch(PromotionEntity target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            target.StartDate = StartDate;
            target.EndDate = EndDate;
            target.StoreId = StoreId;
            target.Name = Name;
            target.Description = Description;
            target.IsActive = IsActive;
            target.IsExclusive = IsExclusive;
            target.EndDate = EndDate;
            target.Priority = Priority;
            target.PredicateVisualTreeSerialized = PredicateVisualTreeSerialized;
            target.PerCustomerLimit = PerCustomerLimit;
            target.TotalLimit = TotalLimit;
            target.PerCustomerLimit = PerCustomerLimit;
            target.IsAllowCombiningWithSelf = IsAllowCombiningWithSelf;

            if (!Stores.IsNullCollection())
            {
                var comparer = AnonymousComparer.Create((PromotionStoreEntity entity) => entity.StoreId);
                Stores.Patch(target.Stores, comparer, (sourceEntity, targetEntity) => targetEntity.StoreId = sourceEntity.StoreId);
            }
        }        
    }
}
