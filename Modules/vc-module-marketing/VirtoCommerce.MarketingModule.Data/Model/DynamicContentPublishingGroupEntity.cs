using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Model
{
    public class DynamicContentPublishingGroupEntity : AuditableEntity
    {
        public DynamicContentPublishingGroupEntity()
        {
            ContentItems = new NullCollection<PublishingGroupContentItemEntity>();
            ContentPlaces = new NullCollection<PublishingGroupContentPlaceEntity>();
        }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        public int Priority { get; set; }

        public bool IsActive { get; set; }

        [StringLength(256)]
        public string StoreId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string ConditionExpression { get; set; }

        public string PredicateVisualTreeSerialized { get; set; }

        #region Navigation Properties
        public virtual ObservableCollection<PublishingGroupContentItemEntity> ContentItems { get; set; }

        public virtual ObservableCollection<PublishingGroupContentPlaceEntity> ContentPlaces { get; set; }
        #endregion


        public virtual DynamicContentPublication ToModel(DynamicContentPublication publication)
        {
            if (publication == null)
                throw new NullReferenceException(nameof(publication));

            publication.Id = Id;
            publication.CreatedBy = CreatedBy;
            publication.CreatedDate = CreatedDate;
            publication.Description = Description;
            publication.ModifiedBy = ModifiedBy;
            publication.ModifiedDate = ModifiedDate;
            publication.Name = Name;
            publication.Priority = Priority;
            publication.IsActive = IsActive;
            publication.StoreId = StoreId;
            publication.StartDate = StartDate;
            publication.EndDate = EndDate;
            publication.PredicateSerialized = ConditionExpression;
            publication.PredicateVisualTreeSerialized = PredicateVisualTreeSerialized;

            if (!string.IsNullOrEmpty(publication.PredicateVisualTreeSerialized))
            {
                //Temporary back data compatibility fix for serialized expressions
                publication.PredicateVisualTreeSerialized = publication.PredicateVisualTreeSerialized.Replace("VirtoCommerce.DynamicExpressionModule.", "VirtoCommerce.DynamicExpressionsModule.");
            }
            if (!string.IsNullOrEmpty(publication.PredicateSerialized))
            {
                //Temporary back data compatibility fix for serialized expressions
                publication.PredicateSerialized = publication.PredicateSerialized.Replace("VirtoCommerce.DynamicExpressionModule.", "VirtoCommerce.DynamicExpressionsModule.");
            }
            if (ContentItems != null)
            {
                publication.ContentItems = ContentItems.Select(x => x.ContentItem.ToModel(AbstractTypeFactory<DynamicContentItem>.TryCreateInstance())).ToList();
            }
            if (ContentPlaces != null)
            {
                publication.ContentPlaces = ContentPlaces.Select(x => x.ContentPlace.ToModel(AbstractTypeFactory<DynamicContentPlace>.TryCreateInstance())).ToList();
            }

            return publication;
        }

        public virtual DynamicContentPublishingGroupEntity FromModel(DynamicContentPublication publication, PrimaryKeyResolvingMap pkMap)
        {
            if (publication == null)
                throw new NullReferenceException(nameof(publication));

            pkMap.AddPair(publication, this);

            Id = publication.Id;
            CreatedBy = publication.CreatedBy;
            CreatedDate = publication.CreatedDate;
            Description = publication.Description;
            ModifiedBy = publication.ModifiedBy;
            ModifiedDate = publication.ModifiedDate;
            Name = publication.Name;
            Priority = publication.Priority;
            IsActive = publication.IsActive;
            StoreId = publication.StoreId;
            StartDate = publication.StartDate;
            EndDate = publication.EndDate;
            ConditionExpression = publication.PredicateSerialized;
            PredicateVisualTreeSerialized = publication.PredicateVisualTreeSerialized;

            if (publication.ContentItems != null)
            {
                ContentItems = new ObservableCollection<PublishingGroupContentItemEntity>(publication.ContentItems.Select(x => new PublishingGroupContentItemEntity { DynamicContentPublishingGroupId = Id, DynamicContentItemId = x.Id }));
            }
            if (publication.ContentPlaces != null)
            {
                ContentPlaces = new ObservableCollection<PublishingGroupContentPlaceEntity>(publication.ContentPlaces.Select(x => new PublishingGroupContentPlaceEntity { DynamicContentPublishingGroupId = Id, DynamicContentPlaceId = x.Id }));
            }
            return this;
        }

        public virtual void Patch(DynamicContentPublishingGroupEntity target)
        {
            if (target == null)
                throw new NullReferenceException(nameof(target));

            target.Description = Description;
            target.Name = Name;
            target.Priority = Priority;
            target.IsActive = IsActive;
            target.StoreId = StoreId;
            target.StartDate = StartDate;
            target.EndDate = EndDate;
            target.ConditionExpression = ConditionExpression;
            target.PredicateVisualTreeSerialized = PredicateVisualTreeSerialized;

            if (!ContentItems.IsNullCollection())
            {
                var itemComparer = AnonymousComparer.Create((PublishingGroupContentItemEntity x) => x.DynamicContentItemId);
                ContentItems.Patch(target.ContentItems, itemComparer, (sourceProperty, targetProperty) => { });
            }

            if (!ContentPlaces.IsNullCollection())
            {
                var itemComparer = AnonymousComparer.Create((PublishingGroupContentPlaceEntity x) => x.DynamicContentPlaceId);
                ContentPlaces.Patch(target.ContentPlaces, itemComparer, (sourceProperty, targetProperty) => { });
            }
        }
    }
}
