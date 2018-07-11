using System.Collections.Generic;
using VirtoCommerce.Tools.Models;
using Xunit;

namespace VirtoCommerce.Tools.Tests
{

    [Trait("Category", "CI")]
    public class CategorySeoTests
    {
        private readonly Store _store = new Store
        {
            Id = "Store1",
            DefaultLanguage = "en-US",
            Languages = new List<string>(new[]
            {
                "en-US",
                "ru-RU",
            }),
            SeoLinksType = SeoLinksType.Collapsed,
        };

        [Fact]
        public void When_HasNoSeoRecords_Expect_Null()
        {
            var result = ((IEnumerable<Outline>)null).GetSeoPath(_store, "en-US", null);
            Assert.Null(result);
        }

        [Fact]
        public void When_HasSeoRecords_Expect_ShortPath()
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
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "category1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "category2"},
                            },
                        }
                    },
                },
            };

            var result = outlines.GetSeoPath(_store, "ru-RU", null);
            Assert.Equal("category2", result);
        }

        [Fact]
        public void When_HasParentSeoRecords_Expect_LongPath()
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
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "grandparent1" },
                                new SeoInfo { StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "grandparent2" },
                            }
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "parent1" },
                                new SeoInfo { StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "parent2" },
                            }
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "category1" },
                                new SeoInfo { StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "category2" },
                            },
                        },
                    },
                },
            };

            var result = outlines.GetSeoPath(_store, "ru-RU", null);
            Assert.Equal("grandparent2/parent2/category2", result);
        }

        [Fact]
        public void When_MissingAnyParentSeoRecord_Expect_Null()
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
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>(),
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "parent1" },
                                new SeoInfo { StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "parent2" },
                            }
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "category1" },
                                new SeoInfo { StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "category2" },
                            },
                        },
                    },
                },
            };

            var result = outlines.GetSeoPath(_store, "ru-RU", null);
            Assert.Null(result);
        }

        [Fact]
        public void When_HasInactiveSeoRecords_Expect_OnlyActiveRecords()
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
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "inactive-parent1", IsActive = false },
                                new SeoInfo { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "active-parent1", IsActive = true },
                                new SeoInfo { StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "inactive-parent2", IsActive = false },
                                new SeoInfo { StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "active-parent2", IsActive = true },
                            }
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "inactive-category1", IsActive = false },
                                new SeoInfo { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "active-category1", IsActive = true },
                                new SeoInfo { StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "inactive-category2", IsActive = false },
                                new SeoInfo { StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "active-category2", IsActive = true },
                            },
                        },
                    },
                },
            };

            var result = outlines.GetSeoPath(_store, "ru-RU", null);
            Assert.Equal("active-parent2/active-category2", result);
        }

        [Fact]
        public void When_HasVirtualParent_Expect_SkipLinkedPhysicalParent()
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
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "virtual-parent1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "virtual-parent2"},
                            }
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            HasVirtualParent = true,
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "physical-parent1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "physical-parent2"},
                            }
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "category1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "category2"},
                            },
                        },
                    },
                },
            };

            var result = outlines.GetSeoPath(_store, "ru-RU", null);
            Assert.Equal("virtual-parent2/category2", result);
        }

        [Fact]
        public void When_IsLinkedToCatalogRoot_Expect_Null()
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
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            HasVirtualParent = true,
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "category1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "category2"},
                            },
                        },
                    },
                },
            };

            var result = outlines.GetSeoPath(_store, "ru-RU", null);
            Assert.Null(result);
        }

        [Fact]
        public void When_ParentIsLinkedToCatalogRoot_Expect_SkipLinkedPhysicalParent()
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
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            HasVirtualParent = true,
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "physical-parent1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "physical-parent2"},
                            }
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "category1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "category2"},
                            },
                        },
                    },
                },
            };

            var result = outlines.GetSeoPath(_store, "ru-RU", null);
            Assert.Equal("category2", result);
        }

        [Fact]
        public void When_LastItemHasVirtualParent_Expect_Null()
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
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "virtual-parent1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "virtual-parent2"},
                            }
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            HasVirtualParent = true,
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "category1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "category2"},
                            },
                        },
                    },
                },
            };

            var result = outlines.GetSeoPath(_store, "ru-RU", null);
            Assert.Null(result);
        }
    }

    [Trait("Category", "CI")]
    public class ProductSeoTests
    {
        private readonly Store _store = new Store
        {
            Id = "Store1",
            DefaultLanguage = "en-US",
            Languages = new List<string>(new[]
            {
                "en-US",
                "ru-RU",
            }),
            SeoLinksType = SeoLinksType.Collapsed,
        };

        [Fact]
        public void When_HasSeoRecordsAndNoCategorySeoRecords_Expect_Null()
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
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "CatalogProduct",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "product2"},
                            },
                        },
                    },
                },
            };

            var result = outlines.GetSeoPath(_store, "ru-RU", null);
            Assert.Null(result);
        }

        [Fact]
        public void When_HasCategorySeoRecords_Expect_LongPath()
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
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "category1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "category2"},
                            },
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "CatalogProduct",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "product2"},
                            },
                        },
                    },
                },
            };

            var result = outlines.GetSeoPath(_store, "ru-RU", null);
            Assert.Equal("category2/product2", result);
        }

        [Fact]
        public void When_HasParentCategorySeoRecords_Expect_LongPath()
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
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "grandparent1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "grandparent2"},
                            }
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "parent1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "parent2"},
                            }
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "category1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "category2"},
                            },
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "CatalogProduct",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "product2"},
                            },
                        },
                    },
                },
            };

            var result = outlines.GetSeoPath(_store, "ru-RU", null);
            Assert.Equal("grandparent2/parent2/category2/product2", result);
        }

        [Fact]
        public void When_MissingAnyParentSeoRecord_Expect_Null()
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
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>(),
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "parent1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "parent2"},
                            }
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "category1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "category2"},
                            },
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "CatalogProduct",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "product2"},
                            },
                        },
                    },
                },
            };

            var result = outlines.GetSeoPath(_store, "ru-RU", null);
            Assert.Null(result);
        }

        [Fact]
        public void When_ProductHasParentCategoryWithVirtualParent_Expect_SkipLinkedPhysicalParent()
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
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "virtual-parent1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "virtual-parent2"},
                            }
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            HasVirtualParent = true,
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "parent1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "parent2"},
                            }
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "CatalogProduct",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "product2"},
                            },
                        },
                    },
                },
            };

            var result = outlines.GetSeoPath(_store, "ru-RU", null);
            Assert.Equal("virtual-parent2/product2", result);
        }

        [Fact]
        public void When_ProductHasVirtualParent_Expect_KeepProduct()
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
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "Category",
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "virtual-parent1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "virtual-parent2"},
                            }
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "CatalogProduct",
                            HasVirtualParent = true,
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "product2"},
                            },
                        },
                    },
                },
            };

            var result = outlines.GetSeoPath(_store, "ru-RU", null);
            Assert.Equal("virtual-parent2/product2", result);
        }

        [Fact]
        public void When_ProductIsLinkedToCatalogRoot_Expect_KeepLinkedProduct()
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
                        },
                        new OutlineItem
                        {
                            SeoObjectType = "CatalogProduct",
                            HasVirtualParent = true,
                            SeoInfos = new List<SeoInfo>
                            {
                                new SeoInfo {StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product1"},
                                new SeoInfo {StoreId = "Store1", LanguageCode = "ru-RU", SemanticUrl = "product2"},
                            },
                        },
                    },
                },
            };

            var result = outlines.GetSeoPath(_store, "ru-RU", null);
            Assert.Equal("product2", result);
        }
    }
}
