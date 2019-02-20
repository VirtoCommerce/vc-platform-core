using System.Collections.Generic;
using Newtonsoft.Json;

namespace VirtoCommerce.CoreModule.Core.Common
{
    public interface ICondition
    {
        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.All)]
        ICollection<ICondition> AvailableChildren { get; set; }
        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.All)]
        ICollection<ICondition> Children { get; set; }

        string Id { get; set; }

        bool Evaluate(IEvaluationContext context);

        IEnumerable<ICondition> GetConditions();
    }
}
