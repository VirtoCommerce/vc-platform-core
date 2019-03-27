using System.Collections.Generic;

namespace VirtoCommerce.CoreModule.Core.Outlines
{
    public interface IHasOutlines
    {
        IList<Outline> Outlines { get; set; }
    }
}
