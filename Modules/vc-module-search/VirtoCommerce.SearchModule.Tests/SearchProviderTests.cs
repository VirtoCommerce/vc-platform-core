using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.SearchModule.Core.Model;
using Xunit;

namespace VirtoCommerce.SearchModule.Tests
{
    [TestCaseOrderer(PriorityTestCaseOrderer.TypeName, PriorityTestCaseOrderer.AssembyName)]
    public abstract class SearchProviderTests : SearchProviderTestsBase
    {
        public const string DocumentType = "item";

        [Fact, Priority(100)]
        public virtual async Task CanAddAndRemoveDocuments()
        {
            var provider = GetSearchProvider();

            // Delete index
            await provider.DeleteIndexAsync(DocumentType);


            // Create index and add documents
            var primaryDocuments = GetPrimaryDocuments();
            var response = await provider.IndexAsync(DocumentType, primaryDocuments);

            Assert.NotNull(response);
            Assert.NotNull(response.Items);
            Assert.Equal(primaryDocuments.Count, response.Items.Count);
            Assert.All(response.Items, i => Assert.True(i.Succeeded));


            // Update index with new fields and add more documents
            var secondaryDocuments = GetSecondaryDocuments();
            response = await provider.IndexAsync(DocumentType, secondaryDocuments);

            Assert.NotNull(response);
            Assert.NotNull(response.Items);
            Assert.Equal(secondaryDocuments.Count, response.Items.Count);
            Assert.All(response.Items, i => Assert.True(i.Succeeded));


            // Remove some documents
            response = await provider.RemoveAsync(DocumentType, new[] { new IndexDocument("Item-7"), new IndexDocument("Item-8") });

            Assert.NotNull(response);
            Assert.NotNull(response.Items);
            Assert.Equal(2, response.Items.Count);
            Assert.All(response.Items, i => Assert.True(i.Succeeded));
        }

        [Fact]
        public virtual async Task CanLimitResults()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                Skip = 4,
                Take = 3,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(2, response.DocumentsCount);
            Assert.Equal(6, response.TotalCount);
        }

        [Fact]
        public virtual async Task CanRetriveStringCollection()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                Take = 10,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            var document = response?.Documents?.FirstOrDefault();
            Assert.NotNull(document);

            var stringCollection = document["catalog"] as object[];
            Assert.NotNull(stringCollection);
            Assert.Equal(2, stringCollection.Length);
        }

        [Fact]
        public virtual async Task CanSortByStringField()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                Sorting = new[]
                {
                    // Sorting by non-existent field should be ignored
                    new SortingField { FieldName = "non-existent-field" },
                    new SortingField { FieldName = "Name" },
                },
                Take = 1,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(1, response.DocumentsCount);

            var productName = response.Documents.First()["name"] as string;
            Assert.Equal("Black Sox", productName);


            request = new SearchRequest
            {
                Sorting = new[] { new SortingField { FieldName = "Name", IsDescending = true } },
                Take = 1,
            };

            response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(1, response.DocumentsCount);

            productName = response.Documents.First()["name"] as string;
            Assert.Equal("Sample Product", productName);
        }

        [Fact]
        public virtual async Task CanSortByNumericField()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                Sorting = new[] { new SortingField { FieldName = "Size", IsDescending = true } },
                Take = 1,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(1, response.DocumentsCount);

            var productName = response.Documents.First()["name"] as string;
            Assert.Equal("Black Sox2", productName);
        }

        [Fact]
        public virtual async Task CanSortByGeoDistance()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                Sorting = new SortingField[]
                {
                    new GeoDistanceSortingField
                    {
                        FieldName = "Location",
                        Location = GeoPoint.TryParse("0, 14")
                    }
                },
                Take = 10,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(6, response.DocumentsCount);

            Assert.Equal("Item-2", response.Documents[0].Id);
            Assert.Equal("Item-3", response.Documents[1].Id);
            Assert.Equal("Item-1", response.Documents[2].Id);
            Assert.Equal("Item-4", response.Documents[3].Id);
            Assert.Equal("Item-5", response.Documents[4].Id);
            Assert.Equal("Item-6", response.Documents[5].Id);
        }

        [Fact]
        public virtual async Task CanSortByGeoDistanceDescending()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                Sorting = new SortingField[]
                {
                    new GeoDistanceSortingField
                    {
                        FieldName = "Location",
                        Location = GeoPoint.Parse("0, 14"),
                        IsDescending = true,
                    }
                },
                Take = 10,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(6, response.DocumentsCount);

            Assert.Equal("Item-6", response.Documents[0].Id);
            Assert.Equal("Item-5", response.Documents[1].Id);
            Assert.Equal("Item-4", response.Documents[2].Id);
            Assert.Equal("Item-1", response.Documents[3].Id);
            Assert.Equal("Item-3", response.Documents[4].Id);
            Assert.Equal("Item-2", response.Documents[5].Id);
        }

        [Fact]
        public virtual async Task CanSearchByKeywords()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                SearchKeywords = " shirt ",
                SearchFields = new[] { "Content" },
                Take = 10,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(3, response.DocumentsCount);


            request = new SearchRequest
            {
                SearchKeywords = "red shirt",
                SearchFields = new[] { "Content" },
                Take = 10,
            };

            response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(2, response.DocumentsCount);
        }

        [Fact]
        public virtual async Task CanFilterByIds()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                Filter = new IdsFilter
                {
                    Values = new[] { "Item-2", "Item-3", "Item-9" },
                },
                Take = 10,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(2, response.DocumentsCount);

            Assert.True(response.Documents.Any(d => d.Id == "Item-2"), "Cannot find 'Item-2'");
            Assert.True(response.Documents.Any(d => d.Id == "Item-3"), "Cannot find 'Item-3'");
        }

        [Fact]
        public virtual async Task CanFilterByTerm()
        {
            var provider = GetSearchProvider();

            // Filtering by non-existent field name leads to empty result
            var request = new SearchRequest
            {
                Filter = new TermFilter
                {
                    FieldName = "non-existent-field",
                    Values = new[] { "value-does-not-matter" }
                },
                Take = 10,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(0, response.DocumentsCount);


            // Filtering by non-existent field value leads to empty result
            request = new SearchRequest
            {
                Filter = new TermFilter
                {
                    FieldName = "Color",
                    Values = new[]
                    {
                        "non-existent-value-1",
                        "non-existent-value-2",
                    }
                },
                Take = 10,
            };

            response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(0, response.DocumentsCount);


            request = new SearchRequest
            {
                Filter = new TermFilter
                {
                    FieldName = "Color",
                    Values = new[]
                    {
                        "Red",
                        "Blue",
                        "Black",
                    }
                },
                Take = 10,
            };

            response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(5, response.DocumentsCount);


            request = new SearchRequest
            {
                Filter = new TermFilter
                {
                    FieldName = "Is",
                    Values = new[]
                    {
                        "Red",
                        "Blue",
                        "Black",
                    }
                },
                Take = 10,
            };

            response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(5, response.DocumentsCount);


            request = new SearchRequest
            {
                Filter = new TermFilter
                {
                    FieldName = "Size",
                    Values = new[]
                    {
                        "1",
                        "2",
                        "3",
                    }
                },
                Take = 10,
            };

            response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(2, response.DocumentsCount);


            request = new SearchRequest
            {
                Filter = new TermFilter
                {
                    FieldName = "Date",
                    Values = new[]
                    {
                        "2017-04-29T15:24:31.180Z",
                        "2017-04-28T15:24:31.180Z",
                        "2017-04-27T15:24:31.180Z",
                    }
                },
                Take = 10,
            };

            response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(2, response.DocumentsCount);
        }

        [Fact]
        public virtual async Task CanFilterByBooleanTerm()
        {
            var provider = GetSearchProvider();


            var request = new SearchRequest
            {
                Filter = new TermFilter
                {
                    FieldName = "HasMultiplePrices",
                    Values = new[] { "tRue" } // Value should be case insensitive
                },
                Take = 10,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(2, response.DocumentsCount);


            request = new SearchRequest
            {
                Filter = new TermFilter
                {
                    FieldName = "HasMultiplePrices",
                    Values = new[] { "fAlse" } // Value should be case insensitive
                },
                Take = 10,
            };

            response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(4, response.DocumentsCount);
        }


        [Fact]
        public virtual async Task CanFilterByRange()
        {
            var provider = GetSearchProvider();

            // Filtering by non-existent field name leads to empty result
            var request = new SearchRequest
            {
                Filter = new RangeFilter
                {
                    FieldName = "non-existent-field",
                    Values = new[]
                    {
                        new RangeFilterValue { Lower = "0", Upper = "4" },
                    }
                },
                Take = 10,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(0, response.DocumentsCount);


            request = new SearchRequest
            {
                Filter = new RangeFilter
                {
                    FieldName = "Size",
                    Values = new[]
                    {
                        new RangeFilterValue { Lower = "0", Upper = "4" },
                        new RangeFilterValue { Lower = "", Upper = "4" },
                        new RangeFilterValue { Lower = null, Upper = "4" },
                        new RangeFilterValue { Lower = "4", Upper = "10" },
                    }
                },
                Take = 10,
            };

            response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(2, response.DocumentsCount);
        }

        [Fact]
        public virtual async Task CanFilterByDateRange()
        {
            var provider = GetSearchProvider();

            var criteria = new SearchRequest { Take = 0, Filter = CreateRangeFilter("Date", "2017-04-23T15:24:31.180Z", "2017-04-28T15:24:31.180Z", true, true) };
            var response = await provider.SearchAsync(DocumentType, criteria);
            Assert.Equal(6, response.TotalCount);

            criteria = new SearchRequest { Take = 0, Filter = CreateRangeFilter("Date", "2017-04-23T15:24:31.180Z", "2017-04-28T15:24:31.180Z", false, true) };
            response = await provider.SearchAsync(DocumentType, criteria);
            Assert.Equal(5, response.TotalCount);

            criteria = new SearchRequest { Take = 0, Filter = CreateRangeFilter("Date", "2017-04-23T15:24:31.180Z", "2017-04-28T15:24:31.180Z", true, false) };
            response = await provider.SearchAsync(DocumentType, criteria);
            Assert.Equal(5, response.TotalCount);

            criteria = new SearchRequest { Take = 0, Filter = CreateRangeFilter("Date", "2017-04-23T15:24:31.180Z", "2017-04-28T15:24:31.180Z", false, false) };
            response = await provider.SearchAsync(DocumentType, criteria);
            Assert.Equal(4, response.TotalCount);

            criteria = new SearchRequest { Take = 0, Filter = CreateRangeFilter("Date", null, "2017-04-28T15:24:31.180Z", true, true) };
            response = await provider.SearchAsync(DocumentType, criteria);
            Assert.Equal(6, response.TotalCount);

            criteria = new SearchRequest { Take = 0, Filter = CreateRangeFilter("Date", null, "2017-04-28T15:24:31.180Z", true, false) };
            response = await provider.SearchAsync(DocumentType, criteria);
            Assert.Equal(5, response.TotalCount);

            criteria = new SearchRequest { Take = 0, Filter = CreateRangeFilter("Date", "2017-04-23T15:24:31.180Z", null, true, false) };
            response = await provider.SearchAsync(DocumentType, criteria);
            Assert.Equal(6, response.TotalCount);

            criteria = new SearchRequest { Take = 0, Filter = CreateRangeFilter("Date", "2017-04-23T15:24:31.180Z", null, false, false) };
            response = await provider.SearchAsync(DocumentType, criteria);
            Assert.Equal(5, response.TotalCount);
        }

        [Fact]
        public virtual async Task CanFilterByGeoDistance()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                Filter = new GeoDistanceFilter
                {
                    FieldName = "Location",
                    Location = GeoPoint.TryParse("0, 14"),
                    Distance = 1110, // less than 10 degrees (1 degree at the equater is about 111 km)
                },
                Take = 10,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(2, response.DocumentsCount);
        }

        [Fact]
        public virtual async Task CanInvertFilterWithNot()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                Filter = new NotFilter
                {
                    ChildFilter = new TermFilter
                    {
                        FieldName = "Size",
                        Values = new[]
                        {
                            "1",
                            "2",
                            "3",
                        }
                    },
                },
                Take = 10,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(4, response.DocumentsCount);


            request = new SearchRequest
            {
                Filter = new NotFilter
                {
                    ChildFilter = new RangeFilter
                    {
                        FieldName = "Size",
                        Values = new[]
                        {
                            new RangeFilterValue { Lower = "0", Upper = "4" },
                            new RangeFilterValue { Lower = "4", Upper = "20" },
                        }
                    },
                },
                Take = 10,
            };

            response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(2, response.DocumentsCount);
        }

        [Fact]
        public virtual async Task CanJoinFiltersWithAnd()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                Filter = new AndFilter
                {
                    ChildFilters = new IFilter[]
                    {
                        new TermFilter
                        {
                            FieldName = "Color",
                            Values = new[]
                            {
                                "Red",
                                "Blue",
                                "Black",
                            }
                        },
                        new RangeFilter
                        {
                            FieldName = "Size",
                            Values = new[]
                            {
                                new RangeFilterValue { Lower = "0", Upper = "4" },
                                new RangeFilterValue { Lower = "4", Upper = "20", IncludeUpper = true },
                            }
                        },
                    }
                },
                Take = 10,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(4, response.DocumentsCount);
        }

        [Fact]
        public virtual async Task CanJoinFiltersWithOr()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                // (Color = Red) OR (Size > 10)
                Filter = new OrFilter
                {
                    ChildFilters = new IFilter[]
                    {
                        new TermFilter
                        {
                            FieldName = "Color",
                            Values = new[]
                            {
                                "Red",
                            }
                        },
                        new RangeFilter
                        {
                            FieldName = "Size",
                            Values = new[]
                            {
                                new RangeFilterValue { Lower = "10" },
                            }
                        },
                    }
                },
                Take = 10,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(4, response.DocumentsCount);
        }

        [Fact]
        public virtual async Task CanFilterByNestedFilters()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                // (Color = Red) OR (NOT(Color = Blue) AND (Size < 20))
                Filter = new OrFilter
                {
                    ChildFilters = new IFilter[]
                    {
                        new TermFilter
                        {
                            FieldName = "Color",
                            Values = new[]
                            {
                                "Red",
                            }
                        },
                        new AndFilter
                        {
                            ChildFilters = new IFilter[]
                            {
                                new NotFilter
                                {
                                    ChildFilter = new TermFilter
                                    {
                                        FieldName = "Color",
                                        Values = new[]
                                        {
                                            "Blue",
                                        }
                                    },
                                },
                                new RangeFilter
                                {
                                    FieldName = "Size",
                                    Values = new[]
                                    {
                                        new RangeFilterValue { Upper = "20" },
                                    }
                                },
                            },
                        },
                    }
                },
                Take = 10,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(4, response.DocumentsCount);
        }

        [Fact]
        public virtual async Task CanLimitFacetSizeForStringField()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                Aggregations = new AggregationRequest[]
                {
                    new TermAggregationRequest { FieldName = "Color", Size = 1 },
                },
                Take = 0,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(0, response.DocumentsCount);
            Assert.Equal(1, response.Aggregations?.Count);

            Assert.Equal(1, GetAggregationValuesCount(response, "Color"));
            Assert.Equal(3, GetAggregationValueCount(response, "Color", "Red"));
        }

        [Fact]
        public virtual async Task CanLimitFacetSizeForNumericField()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                Aggregations = new AggregationRequest[]
                {
                    new TermAggregationRequest { FieldName = "Size", Size = 1 },
                },
                Take = 0,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(0, response.DocumentsCount);
            Assert.Equal(1, response.Aggregations?.Count);

            Assert.Equal(1, GetAggregationValuesCount(response, "Size"));
            Assert.Equal(2, GetAggregationValueCount(response, "Size", "10"));
        }

        [Fact]
        public virtual async Task CanGetAllFacetValuesForStringField()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                Aggregations = new AggregationRequest[]
                {
                    // Facets for non-existent fields should be ignored
                    new TermAggregationRequest { FieldName = "non-existent-field", Size = 0 },
                    new TermAggregationRequest { FieldName = "Color", Size = 0 },
                },
                Take = 0,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(0, response.DocumentsCount);
            Assert.Equal(1, response.Aggregations?.Count);

            Assert.Equal(4, GetAggregationValuesCount(response, "Color"));
            Assert.Equal(3, GetAggregationValueCount(response, "Color", "Red"));
            Assert.Equal(1, GetAggregationValueCount(response, "Color", "Black"));
            Assert.Equal(1, GetAggregationValueCount(response, "Color", "Blue"));
            Assert.Equal(1, GetAggregationValueCount(response, "Color", "Silver"));
        }

        [Fact]
        public virtual async Task CanGetAllFacetValuesForNumericField()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                Aggregations = new AggregationRequest[]
                {
                    new TermAggregationRequest { FieldName = "Size", Size = 0 },
                },
                Take = 0,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(0, response.DocumentsCount);
            Assert.Equal(1, response.Aggregations?.Count);

            Assert.Equal(5, GetAggregationValuesCount(response, "Size"));
            Assert.Equal(1, GetAggregationValueCount(response, "Size", "2"));
            Assert.Equal(1, GetAggregationValueCount(response, "Size", "3"));
            Assert.Equal(1, GetAggregationValueCount(response, "Size", "4"));
            Assert.Equal(2, GetAggregationValueCount(response, "Size", "10"));
            Assert.Equal(1, GetAggregationValueCount(response, "Size", "20"));
        }

        [Fact]
        public virtual async Task CanGetSpecificFacetValuesForStringField()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                Aggregations = new AggregationRequest[]
                {
                    new TermAggregationRequest
                    {
                        // Facets for non-existent fields should be ignored
                        FieldName = "non-existent-field",
                        Values = new[] { "Red" }
                    },
                    new TermAggregationRequest
                    {
                        FieldName = "Color",
                        Values = new[] { "Red", "Blue", "White" }
                    },
                },
                Take = 0,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(0, response.DocumentsCount);
            Assert.Equal(1, response.Aggregations?.Count);

            Assert.Equal(2, GetAggregationValuesCount(response, "Color"));
            Assert.Equal(3, GetAggregationValueCount(response, "Color", "Red"));
            Assert.Equal(1, GetAggregationValueCount(response, "Color", "Blue"));
        }

        [Fact]
        public virtual async Task CanGetSpecificFacetValuesForNumericField()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                Aggregations = new AggregationRequest[]
                {
                    new TermAggregationRequest
                    {
                        FieldName = "Size",
                        Values = new[] { "3", "4", "5" }
                    },
                },
                Take = 0,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(0, response.DocumentsCount);
            Assert.Equal(1, response.Aggregations?.Count);

            Assert.Equal(2, GetAggregationValuesCount(response, "Size"));
            Assert.Equal(1, GetAggregationValueCount(response, "Size", "3"));
            Assert.Equal(1, GetAggregationValueCount(response, "Size", "4"));
        }

        [Fact]
        public virtual async Task CanGetRangeFacets()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                Aggregations = new AggregationRequest[]
                {
                    new RangeAggregationRequest
                    {
                        // Facets for non-existent fields should be ignored
                        FieldName = "non-existent-field",
                        Values = new[] { new RangeAggregationRequestValue { Id = "5_to_20", Lower = "5", Upper = "20" } }
                    },
                    new RangeAggregationRequest
                    {
                        FieldName = "Size",
                        Values = new[]
                        {
                            new RangeAggregationRequestValue { Id = "5_to_20", Lower = "5", Upper = "20" },
                            new RangeAggregationRequestValue { Id = "0_to_5", Lower = "0", Upper = "5" },
                        }
                    },
                }
            };

            var response = await provider.SearchAsync(DocumentType, request);

            var size0To5Count = GetAggregationValueCount(response, "Size", "0_to_5");
            Assert.Equal(3, size0To5Count);

            var size5To10Count = GetAggregationValueCount(response, "Size", "5_to_20");
            Assert.Equal(2, size5To10Count);
        }

        [Fact]
        public virtual async Task CanGetAllFacetValuesWhenRequestFilterIsApplied()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                Aggregations = new AggregationRequest[]
                {
                    new TermAggregationRequest
                    {
                        FieldName = "Color",
                        Values = new[] { "Red", "Blue", "Black", "Silver" }
                    },
                },
                Filter = new AndFilter
                {
                    ChildFilters = new IFilter[]
                    {
                        new TermFilter
                        {
                            FieldName = "Color",
                            Values = new[] { "Red", "Blue" }
                        },
                        new TermFilter
                        {
                            FieldName = "Size",
                            Values = new[] { "2", "4" }
                        },
                    }
                },
                Take = 10,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(2, response.DocumentsCount);
            Assert.Equal(1, response.Aggregations?.Count);

            Assert.Equal(4, GetAggregationValuesCount(response, "Color"));
            Assert.Equal(3, GetAggregationValueCount(response, "Color", "Red"));
            Assert.Equal(1, GetAggregationValueCount(response, "Color", "Black"));
            Assert.Equal(1, GetAggregationValueCount(response, "Color", "Blue"));
            Assert.Equal(1, GetAggregationValueCount(response, "Color", "Silver"));
        }

        [Fact]
        public async Task CanGetFacetWithFilterOnly()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                Aggregations = new AggregationRequest[]
                {
                    new TermAggregationRequest
                    {
                        Id = "Filtered-Aggregation",
                        Filter = new TermFilter
                        {
                            FieldName = "Size",
                            Values = new[] { "10" }
                        },
                    },
                },
                Take = 0,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(0, response.DocumentsCount);
            Assert.Equal(1, response.Aggregations?.Count);

            Assert.Equal(1, GetAggregationValuesCount(response, "Filtered-Aggregation"));
            Assert.Equal(2, GetAggregationValueCount(response, "Filtered-Aggregation", "Filtered-Aggregation"));
        }

        [Fact]
        public virtual async Task CanApplyDifferentFiltersToFacetsAndRequest()
        {
            var provider = GetSearchProvider();

            var request = new SearchRequest
            {
                Aggregations = new AggregationRequest[]
                {
                    new TermAggregationRequest
                    {
                        FieldName = "Color",
                        Values = new[] { "Red", "Blue", "Black", "Silver" },
                        Filter = new TermFilter
                        {
                            FieldName = "Size",
                            Values = new[] { "10" }
                        },
                    },
                },
                Filter = new AndFilter
                {
                    ChildFilters = new IFilter[]
                    {
                        new TermFilter
                        {
                            FieldName = "Color",
                            Values = new[] { "Red", "Blue" }
                        },
                        new TermFilter
                        {
                            FieldName = "Size",
                            Values = new[] { "2", "4" }
                        },
                    }
                },
                Take = 10,
            };

            var response = await provider.SearchAsync(DocumentType, request);

            Assert.Equal(2, response.DocumentsCount);
            Assert.Equal(1, response.Aggregations?.Count);

            Assert.Equal(2, GetAggregationValuesCount(response, "Color"));
            Assert.Equal(0, GetAggregationValueCount(response, "Color", "Red"));
            Assert.Equal(1, GetAggregationValueCount(response, "Color", "Black"));
            Assert.Equal(1, GetAggregationValueCount(response, "Color", "Blue"));
            Assert.Equal(0, GetAggregationValueCount(response, "Color", "Silver"));
        }
    }
}
