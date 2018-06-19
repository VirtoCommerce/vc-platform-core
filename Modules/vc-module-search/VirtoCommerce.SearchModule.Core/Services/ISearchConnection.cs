namespace VirtoCommerce.SearchModule.Core.Services
{
    public interface ISearchConnection
    {
        string Provider { get; }
        string Scope { get; }
        string this[string name] { get; }
    }
}
