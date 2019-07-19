using System;
using Xunit;

namespace VirtoCommerce.OrdersModule.Tests
{
    [Trait("Category", "CI")]
    public class CheckingResponseGroup
    {
        //ToDo
        //private static Permission[] PreparePermissions(bool withPrices = false)
        //{
        //    var permissions = new List<Permission>
        //    {
        //        new Permission {Id = ModuleConstants.Security.Permissions.Read,},
        //        new Permission {Id = ModuleConstants.Security.Permissions.Access,}
        //    };

        //    if (withPrices)
        //    {
        //        permissions.Add(new Permission { Id = ModuleConstants.Security.Permissions.ReadPrices });
        //    }

        //    return permissions.ToArray();
        //}

        //[Theory]
        //[InlineData("WithItems, WithInPayments, WithShipments, WithAddresses, WithDiscounts", null)]
        //[InlineData("scope1,scope2", "scope1,scope2")]
        //[InlineData("scope1,scope2", "WithPrices,scope1,scope2")]
        //[InlineData("scope1", "scope1")]
        //public void CanCheckPermissionsWithNoPrices(string expected, string respGroup)
        //{
        //    var permissions = PreparePermissions();
        //    Assert.Equal(expected, OrderReadPricesPermission.ApplyResponseGroupFiltering(permissions, respGroup));
        //}

        //[Theory]
        //[InlineData(null, null)]
        //[InlineData("scope1,scope2", "scope1,scope2")]
        //[InlineData("WithPrices,scope1,scope2", "WithPrices,scope1,scope2")]
        //[InlineData("scope1", "scope1")]
        //public void CanCheckPermissionsWithPrices(string expected, string respGroup)
        //{
        //    var permissions = PreparePermissions(true);
        //    Assert.Equal(expected, OrderReadPricesPermission.ApplyResponseGroupFiltering(permissions, respGroup));
        //}

        //[Theory]
        //[InlineData(null, null)]
        //[InlineData("scope1,scope2", "scope1,scope2")]
        //[InlineData("WithPrices,scope1,scope2", "WithPrices,scope1,scope2")]
        //[InlineData("scope1", "scope1")]
        //public void CanCheckPermissionsNoPermissions(string expected, string respGroup)
        //{
        //    var permissions = new Permission[0];
        //    Assert.Equal(expected, OrderReadPricesPermission.ApplyResponseGroupFiltering(permissions, respGroup));
        //}
    }
}
