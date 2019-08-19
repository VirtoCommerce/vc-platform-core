namespace VirtoCommerce.ExportModule.Core.Model
{
    /// <summary>
    /// Common (default) set of properties for universal viewing.
    /// </summary>
    public interface IExportViewable
    {
        string Name { get; set; }
        string Code { get; set; }
        string ImageUrl { get; set; }
        string Parent { get; set; }
        string Type { get; set; }
    }
}
