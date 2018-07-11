namespace VirtoCommerce.CoreModule.Core.Tax
{
    public interface ITaxable
    {
        /// <summary>
        /// Tax category or type
        /// </summary>
        string TaxType { get; set; }

        decimal TaxTotal { get; }
        decimal TaxPercentRate { get; }
    }
}
