using System.Linq;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.CoreModule.Core.Conditions.Browse;
using VirtoCommerce.CoreModule.Core.Conditions.GeoConditions;

namespace VirtoCommerce.MarketingModule.Core.Model.DynamicContent
{
    public class DynamicContentConditionTreePrototype : ConditionTree
    {
        public DynamicContentConditionTreePrototype()
        {
            WithAvailConditions(
              new BlockContentCondition()
                  .WithAvailConditions(
                     new ConditionGeoTimeZone(),
                     new ConditionGeoZipCode(),
                     new ConditionStoreSearchedPhrase(),
                     new ConditionAgeIs(),
                     new ConditionGenderIs(),
                     new ConditionGeoCity(),
                     new ConditionGeoCountry(),
                     new ConditionGeoState(),
                     new ConditionLanguageIs(),
                     new UserGroupsContainsCondition()
                   )
             );
            Children = AvailableChildren.ToList();
        }
    }
}
