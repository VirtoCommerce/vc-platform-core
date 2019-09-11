using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Model
{
    public class EditorialReviewEntity : AuditableEntity
    {
        public int Priority { get; set; }

        [StringLength(128)]
        public string Source { get; set; }

        public string Content { get; set; }

        [Required]
        public int ReviewState { get; set; }

        public string Comments { get; set; }

        [StringLength(64)]
        public string Locale { get; set; }

        #region Navigation Properties

        public string ItemId { get; set; }
        public ItemEntity CatalogItem { get; set; }

        #endregion

        public virtual EditorialReview ToModel(EditorialReview review)
        {
            if (review == null)
                throw new ArgumentNullException(nameof(review));

            review.Id = Id;
            review.Content = Content;
            review.CreatedBy = CreatedBy;
            review.CreatedDate = CreatedDate;
            review.ModifiedBy = ModifiedBy;
            review.ModifiedDate = ModifiedDate;

            review.LanguageCode = Locale;
            review.ReviewType = Source;

            return review;
        }

        public virtual EditorialReviewEntity FromModel(EditorialReview review, PrimaryKeyResolvingMap pkMap)
        {
            if (review == null)
                throw new ArgumentNullException(nameof(review));

            pkMap.AddPair(review, this);

            Id = review.Id;
            Content = review.Content;
            CreatedBy = review.CreatedBy;
            CreatedDate = review.CreatedDate;
            ModifiedBy = review.ModifiedBy;
            ModifiedDate = review.ModifiedDate;

            Locale = review.LanguageCode;
            Source = review.ReviewType;

            return this;
        }

        public virtual void Patch(EditorialReviewEntity target)
        {
            target.Content = Content;
            target.Locale = Locale;
            target.Source = Source;
        }
    }
}
