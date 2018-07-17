using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using VirtoCommerce.CustomerModule.Data.Common;
using VirtoCommerce.CustomerModule.Web.Controllers.Api;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Domain.Customer.Model;
using Xunit;

namespace VirtoCommerce.CustomerModule.Test
{
    public class TopologicalSortTests
    {
        [Fact]
        public void SearchContactsTest()
        {
            //
            // digraph G {
            //   "7"  -> "11"
            //   "7"  -> "8"
            //   "5"  -> "11"
            //   "3"  -> "8"
            //   "3"  -> "10"
            //   "11" -> "2"
            //   "11" -> "9"
            //   "11" -> "10"
            //   "8"  -> "9"
            // }

            var ret = TopologicalSort.Sort(
                new HashSet<int>(new[] {1, 7, 5, 3, 8, 11, 2, 9, 10, 2 }),
                new HashSet<Tuple<int, int>>(
                    new[] {
                        Tuple.Create(7, 11),
                        Tuple.Create(7, 8),
                        Tuple.Create(5, 11),
                        Tuple.Create(3, 8),
                        Tuple.Create(3, 10),
                        Tuple.Create(11, 2),
                        Tuple.Create(11, 9),
                        Tuple.Create(11, 10),
                        Tuple.Create(8, 9)
                    }
                )
            ).ToList();
            System.Diagnostics.Debug.Assert(ret.SequenceEqual(new[] { 1, 7, 5, 11, 2, 3, 8, 9, 10 }));
        }
    }
}
