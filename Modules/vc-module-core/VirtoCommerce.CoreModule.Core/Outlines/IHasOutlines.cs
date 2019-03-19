using System.Collections.Generic;

namespace VirtoCommerce.CoreModule.Core.Outlines
{
    public interface IHasOutlines
    {
        ICollection<Outline> Outlines { get; set; }
    }
}
