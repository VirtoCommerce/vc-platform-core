angular.module('virtoCommerce.customerModule')
.controller('virtoCommerce.customerModule.vendorDetailController', ['$scope', function ($scope) {
    var blade = $scope.blade;

    if (blade.isNew) {
        blade.title = 'customer.blades.vendor-detail.title-new';
        blade.currentEntity = angular.extend({
            seoInfos: []
        }, blade.currentEntity);

        blade.fillDynamicProperties();
    } else {
        blade.subtitle = 'customer.blades.vendor-detail.subtitle';
    }
}]);