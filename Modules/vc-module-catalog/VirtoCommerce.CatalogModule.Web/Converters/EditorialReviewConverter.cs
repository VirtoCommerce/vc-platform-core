using moduleModel = VirtoCommerce.CatalogModule.Core.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Converters
{
    public static class EditorialReviewConverter
    {
        public static webModel.EditorialReview ToWebModel(this moduleModel.EditorialReview review)
        {
            return new webModel.EditorialReview
            {
                Content = review.Content,
                Id = review.Id,
                IsInherited = review.IsInherited,
                LanguageCode = review.LanguageCode,
                ReviewType = review.ReviewType
            };
        }

        public static moduleModel.EditorialReview ToCoreModel(this webModel.EditorialReview review)
        {
            return new moduleModel.EditorialReview
            {
                Content = review.Content,
                Id = review.Id,
                IsInherited = review.IsInherited,
                LanguageCode = review.LanguageCode,
                ReviewType = review.ReviewType
            };
        }
    }
}
