﻿angular.module('virtoCommerce.marketingModule')
.controller('virtoCommerce.marketingModule.dashboard.promotionsWidgetController', ['$scope', '$state', 'virtoCommerce.marketingModule.promotions', function ($scope, $state, promotions) {
    $scope.data = { count: '', descr: 'Active Promotions' };

    $scope.widgetAction = function () {
        $state.go('workspace.marketing');
    };

    promotions.search({ responseGroup: 'withPromotions', count: 1000 }, function (data) {
        var selection = _.where(data.results, { isActive: true });
        $scope.data.count = selection.length;
    });
}]);