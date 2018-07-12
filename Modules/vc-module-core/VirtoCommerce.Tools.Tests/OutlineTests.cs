using System.Collections.Generic;
using VirtoCommerce.Tools.Models;
using Xunit;

namespace VirtoCommerce.Tools.Tests
{
    [Trait("Category", "CI")]
    public class OutlineTests
    {
        [Fact]
        public void When_HasMultipleOutlines_Expect_PathForGivenCatalog()
        {
            var outlines = new List<Outline>
            {
                new Outline
                {
                    Items = new List<OutlineItem>
                    {
                        new OutlineItem
                        {
                            SeoObjectType = "Catalog",
                            Id = "catalog1",
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            Id = "parent1",
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            Id = "category1",
                        },
                    },
                },
                new Outline
                {
                    Items = new List<OutlineItem>
                    {
                        new OutlineItem
                        {
                            SeoObjectType = "Catalog",
                            Id = "catalog2",
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            Id = "parent2",
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            Id = "category2",
                        },
                    },
                },
            };

            var result = outlines.GetOutlinePath("catalog2");
            Assert.Equal("parent2/category2", result);
        }
    }
}
