using System;

namespace VirtoCommerce.SearchModule.Tests
{
    public class PriorityAttribute : Attribute
    {
        public PriorityAttribute(int priority)
        {
            Priority = priority;
        }

        public int Priority { get; }
    }
}
