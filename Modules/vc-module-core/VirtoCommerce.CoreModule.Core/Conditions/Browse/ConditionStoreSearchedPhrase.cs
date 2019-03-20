using VirtoCommerce.CoreModule.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Conditions.Browse
{
    //Shopper searched for phrase [] in store
    public class ConditionStoreSearchedPhrase : MatchedConditionBase
    {
        public override bool Evaluate(IEvaluationContext context)
        {
            var result = false;
            if (context is EvaluationContextBase evaluationContext)
            {
                result = UseMatchedCondition(evaluationContext.ShopperSearchedPhraseInStore);
            }

            return result;
        }
    }
}
