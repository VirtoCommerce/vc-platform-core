using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.CoreModule.Core.Conditions.Browse;
using VirtoCommerce.CoreModule.Core.Conditions.GeoConditions;

namespace VirtoCommerce.MarketingModule.Core.Model.DynamicContent
{
    public class BlockContentCondition : BlockConditionAndOr
    {
        public override IEnumerable<IConditionTree> AvailableChildren
        {
            get
            {
                yield return new ConditionGeoTimeZone();
                yield return new ConditionGeoZipCode();
                yield return new ConditionStoreSearchedPhrase();
                yield return new ConditionAgeIs();
                yield return new ConditionGenderIs();
                yield return new ConditionGeoCity();
                yield return new ConditionGeoCountry();
                yield return new ConditionGeoState();
                yield return new ConditionLanguageIs();
                yield return new UserGroupsContainsCondition();
               
            }
        }
    }
}
