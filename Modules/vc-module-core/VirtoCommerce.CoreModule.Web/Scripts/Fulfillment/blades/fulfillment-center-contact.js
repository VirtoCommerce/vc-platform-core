angular.module('virtoCommerce.coreModule.fulfillment')
.controller('virtoCommerce.coreModule.fulfillment.fulfillmentCenterContactController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.common.countries', function ($scope, bladeNavigationService, countries) {
    var blade = $scope.blade;

    $scope.saveChanges = function () {
        angular.copy(blade.currentEntity, blade.origEntity);
        $scope.bladeClose();
    };

    $scope.setForm = function (form) {
        $scope.formScope = form;
    }
    
    $scope.cancelChanges = function () {
        $scope.bladeClose();
    }

    $scope.blade.headIcon = 'fa-wrench';

    $scope.$watch('blade.currentEntity.countryCode', function (countryCode) {
        var country;
        if (countryCode && (country = _.findWhere($scope.countries, { id: countryCode }))) {
            blade.currentEntity.countryName = country.name;
        }
    });
   
    blade.isLoading = false;
    blade.currentEntity = angular.copy(blade.data);
    blade.origEntity = blade.data;
    $scope.countries = countries.query();
}]);