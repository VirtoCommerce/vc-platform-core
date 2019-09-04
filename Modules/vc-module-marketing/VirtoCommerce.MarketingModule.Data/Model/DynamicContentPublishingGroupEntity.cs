using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Model.DynamicContent;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketingModule.Data.Model
{
    public class DynamicContentPublishingGroupEntity : AuditableEntity, IHasOuterId
    {
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

        [StringLength(128)]
        public string OuterId { get; set; }

        #region Navigation Properties

        public virtual ObservableCollection<PublishingGroupContentItemEntity> ContentItems { get; set; }
            = new NullCollection<PublishingGroupContentItemEntity>();

        public virtual ObservableCollection<PublishingGroupContentPlaceEntity> ContentPlaces { get; set; }
            = new NullCollection<PublishingGroupContentPlaceEntity>();

        #endregion

        public virtual DynamicContentPublication ToModel(DynamicContentPublication publication)
        {
            if (publication == null)
            {
                throw new ArgumentNullException(nameof(publication));
            }

            publication.Id = Id;
            publication.CreatedBy = CreatedBy;
            publication.CreatedDate = CreatedDate;
            publication.ModifiedBy = ModifiedBy;
            publication.ModifiedDate = ModifiedDate;
            publication.OuterId = OuterId;

            publication.Name = Name;
            publication.Priority = Priority;
            publication.IsActive = IsActive;
            publication.StoreId = StoreId;
            publication.StartDate = StartDate;
            publication.EndDate = EndDate;
            publication.Description = Description;

            if (ContentItems != null)
            {
                //TODO
                publication.ContentItems = ContentItems.Where(ci => ci.ContentItem != null).Select(x => x.ContentItem.ToModel(AbstractTypeFactory<DynamicContentItem>.TryCreateInstance())).ToList();
            }
            if (ContentPlaces != null)
            {
                //TODO
                publication.ContentPlaces = ContentPlaces.Where(ci => ci.ContentPlace != null).Select(x => x.ContentPlace.ToModel(AbstractTypeFactory<DynamicContentPlace>.TryCreateInstance())).ToList();
            }

            publication.DynamicExpression = AbstractTypeFactory<DynamicContentConditionTree>.TryCreateInstance();
            if (PredicateVisualTreeSerialized != null)
            {
                publication.DynamicExpression = JsonConvert.DeserializeObject<DynamicContentConditionTree>(PredicateVisualTreeSerialized, new ConditionJsonConverter());
            }

            return publication;
        }

        public virtual DynamicContentPublishingGroupEntity FromModel(DynamicContentPublication publication, PrimaryKeyResolvingMap pkMap)
        {
            if (publication == null)
            {
                throw new ArgumentNullException(nameof(publication));
            }

            pkMap.AddPair(publication, this);

            Id = publication.Id;
            CreatedBy = publication.CreatedBy;
            CreatedDate = publication.CreatedDate;
            ModifiedBy = publication.ModifiedBy;
            ModifiedDate = publication.ModifiedDate;
            OuterId = publication.OuterId;

            Name = publication.Name;
            Priority = publication.Priority;
            IsActive = publication.IsActive;
            StoreId = publication.StoreId;
            StartDate = publication.StartDate;
            EndDate = publication.EndDate;
 
            Description = publication.Description;

            if (publication.ContentItems != null)
            {
                ContentItems = new ObservableCollection<PublishingGroupContentItemEntity>(publication.ContentItems.Select(x => new PublishingGroupContentItemEntity { DynamicContentPublishingGroupId = Id, DynamicContentItemId = x.Id }));
            }
            if (publication.ContentPlaces != null)
            {
                ContentPlaces = new ObservableCollection<PublishingGroupContentPlaceEntity>(publication.ContentPlaces.Select(x => new PublishingGroupContentPlaceEntity { DynamicContentPublishingGroupId = Id, DynamicContentPlaceId = x.Id }));
            }
            if (publication.DynamicExpression != null)
            {
                PredicateVisualTreeSerialized = JsonConvert.SerializeObject(publication.DynamicExpression, new ConditionJsonConverter(doNotSerializeAvailCondition: true));
            }
            return this;
        }

        public virtual void Patch(DynamicContentPublishingGroupEntity target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            target.Description = Description;
            target.Name = Name;
            target.Priority = Priority;
            target.IsActive = IsActive;
            target.StoreId = StoreId;
            target.StartDate = StartDate;
            target.EndDate = EndDate;
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
