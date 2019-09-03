using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Tests.ComplexExportPagedDataSourceTests.Mocks
{
    public class ComplexSearchEntity : IExportable
    {
        public string Id { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
