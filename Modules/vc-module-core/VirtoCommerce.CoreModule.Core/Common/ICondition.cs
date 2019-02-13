namespace VirtoCommerce.CoreModule.Core.Common
{
    public interface ICondition
    {
        bool Evaluate(IEvaluationContext context);
    }
}
