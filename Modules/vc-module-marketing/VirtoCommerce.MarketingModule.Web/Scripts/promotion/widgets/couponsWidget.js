angular.module('virtoCommerce.marketingModule')
.controller('virtoCommerce.marketingModule.couponsWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.marketingModule.promotions', function ($scope, bladeNavigationService, promotionsApi) {
    var blade = $scope.blade;

    blade.couponCount = function () {
        if (!blade.isNew) {
            promotionsApi.searchCoupons({ promotionId: blade.currentEntityId, skip: 0, take: 1 }, function (response) {
                blade.totalCouponsCount = response.totalCount;
            });
        }
    }

    blade.couponCount();

    $scope.$on('coupon-import-finished', function (event) {
        blade.couponCount();
    });

    $scope.openBlade = function () {
        var newBlade = {
            id: 'coupons',
            title: 'marketing.blades.promotion-detail.toolbar.coupons',
            promotionId: blade.currentEntity.id,
            promotion: blade.currentEntity,
            controller: 'virtoCommerce.marketingModule.couponListController',
            template: 'Modules/$(VirtoCommerce.Marketing)/Scripts/promotion/blades/coupon-list.tpl.html'
        }
        bladeNavigationService.showBlade(newBlade, blade);
    }
}]);