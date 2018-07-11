namespace VirtoCommerce.CoreModule.Core.Common
{
    public interface IHasDimension
    {
        string WeightUnit { get; set; }
        decimal? Weight { get; set; }

        string MeasureUnit { get; set; }
        decimal? Height { get; set; }
        decimal? Length { get; set; }
        decimal? Width { get; set; }

    }
}
