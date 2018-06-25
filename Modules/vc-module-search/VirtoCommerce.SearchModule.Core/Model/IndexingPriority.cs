namespace VirtoCommerce.SearchModule.Core.Model
{
    /// <summary>
    /// Priority for index work.
    /// </summary>
    public enum IndexingPriority
    {
        Background = 0,
        NearRealTime = 1,

        Default = Background
    }
}
