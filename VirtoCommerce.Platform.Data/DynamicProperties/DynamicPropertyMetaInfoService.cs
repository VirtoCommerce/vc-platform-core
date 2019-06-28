using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.Platform.Data.DynamicProperties
{
    public class DynamicPropertyMetaInfoService : IDynamicPropertyMetaInfoService
    {
        private readonly IDynamicPropertyService _dynamicPropertyService;

        public DynamicPropertyMetaInfoService(IDynamicPropertyService dynamicPropertyService)
        {
            _dynamicPropertyService = dynamicPropertyService;
        }

        public virtual async Task ResolveMetaInfoAsync(params IHasDynamicProperties[] owners)
        {
            if (owners == null)
            {
                throw new ArgumentNullException(nameof(owners));
            }

            var propOwners = owners.SelectMany(x => x.GetFlatObjectsListWithInterface<IHasDynamicProperties>()).ToArray();

            var objectTypes = propOwners.Select(x => x.ObjectType ?? x.GetType().FullName).Distinct().ToArray();
            var dynamicProperties = await _dynamicPropertyService.GetObjectDynamicPropertiesByObjectTypesAsync(objectTypes);

            foreach (var propOwner in propOwners)
            {
                var objectType = propOwner.ObjectType ?? propOwner.GetType().FullName;

                //Filter only properties with belongs to concrete type
                var dynamicObjectProps = dynamicProperties
                    .Where(x => x.ObjectType.EqualsInvariant(objectType))
                    .Select(prop =>
                    {
                        prop.ObjectId = propOwner.Id;

                        var ownerDynamicProperty = propOwner.DynamicProperties.FirstOrDefault(x => x.Id.EqualsInvariant(prop.Id));
                        prop.Values = ownerDynamicProperty == null ? new List<DynamicPropertyObjectValue>() : ownerDynamicProperty.Values;

                        return prop;
                    })
                    .ToList();

                propOwner.DynamicProperties = dynamicObjectProps;
            }
        }
    }
}
