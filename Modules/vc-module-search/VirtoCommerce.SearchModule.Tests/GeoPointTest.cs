using System;
using VirtoCommerce.SearchModule.Core.Model;
using Xunit;

namespace VirtoCommerce.SearchModule.Tests
{
    public class GeoPointTest
    {

        [Fact]
        public virtual void CanTryParseSortString()
        {
            //TODO after will add ProductSearchCriteria
            //var geoSortInfo = new GeoSortInfo
            //{
            //    SortColumn = "location2",
            //    GeoPoint = new GeoPoint
            //    {
            //        Latitude = 54.7322,
            //        Longitude = 20.5258
            //    },
            //    SortDirection = SortDirection.Ascending
            //};

            //var searchCriteria = new ProductSearchCriteria
            //{
            //    Sort = "location2(54.7322,20.5258):asc;name:desc"
            //};

            //var result = searchCriteria.SortInfos;

            //Assert.Equal(2, result.Length);
            //Assert.Equal(geoSortInfo, result[0]);
        }

        [CLSCompliant(false)]
        [Theory]
        [InlineData(90, -127.554334, "+90.0, -127.554334")]
        [InlineData(45, 180, "45, 180")]
        [InlineData(90, -180, "90, -180")]
        [InlineData(-90.000, -180.0000, "-90.000, -180.0000")]
        [InlineData(90, 180, "+90, +180")]
        [InlineData(47.1231231, 179.9999999, "47.1231231, 179.9999999")]
        [InlineData(78.7777778, 78.7777778, "78.7777777888, 78.7777777888")]
        [InlineData(78.7777777, 78.7777777, "78.77777774444, 78.77777774444")]
        public virtual void CanTryParseGeoPoint( double lat, double lot, string point)
        {
            var result = GeoPoint.TryParse(point);

            var geoPoint = new GeoPoint { Latitude = lat, Longitude = lot };

            Assert.Equal(geoPoint, result);
        }

        [CLSCompliant(false)]
        [Theory]
        [InlineData("+90.1, -100.111")]
        [InlineData("-91, 123.456")]
        [InlineData("045, 180")]
        public virtual void CantTryParseGeoPoint(string point)
        {
            var result = GeoPoint.TryParse(point);
            Assert.Null(result);
        }
    }
}
