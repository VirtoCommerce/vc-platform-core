using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.CoreModule.Core.Conditions.Browse;
using VirtoCommerce.CoreModule.Core.Conditions.GeoConditions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.PricingModule.Core.Model
{
    public class BlockPricingCondition : BlockConditionAndOr
    {
        public override IEnumerable<IConditionTree> AvailableChildren
        {
            get
            {
                yield return AbstractTypeFactory<ConditionGeoTimeZone>.TryCreateInstance();
                yield return AbstractTypeFactory<ConditionGeoZipCode>.TryCreateInstance();
                yield return AbstractTypeFactory<ConditionStoreSearchedPhrase>.TryCreateInstance();
                yield return AbstractTypeFactory<ConditionAgeIs>.TryCreateInstance();
                yield return AbstractTypeFactory<ConditionGenderIs>.TryCreateInstance();
                yield return AbstractTypeFactory<ConditionGeoCity>.TryCreateInstance();
                yield return AbstractTypeFactory<ConditionGeoCountry>.TryCreateInstance();
                yield return AbstractTypeFactory<ConditionGeoState>.TryCreateInstance();
                yield return AbstractTypeFactory<ConditionLanguageIs>.TryCreateInstance();
                yield return AbstractTypeFactory<UserGroupsContainsCondition>.TryCreateInstance();
            }
        }
    }
}
