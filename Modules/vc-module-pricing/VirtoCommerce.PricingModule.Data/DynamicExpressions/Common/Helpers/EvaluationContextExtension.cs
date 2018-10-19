using System;
using System.Linq;
using VirtoCommerce.PricingModule.Core.Model;

namespace VirtoCommerce.PricingModule.Data.DynamicExpressions.Common.Helpers
{
    public static class EvaluationContextExtension
    {
        #region Dynamic expression evaluation helper methods
        public static bool UserGroupsContains(this EvaluationContextBase context, string group)
        {
            var retVal = context.UserGroups != null;
            if (retVal)
            {
                retVal = context.UserGroups.Any(x => string.Equals(x, group, StringComparison.InvariantCultureIgnoreCase));
            }
            return retVal;
        }
        #endregion
    }
}
