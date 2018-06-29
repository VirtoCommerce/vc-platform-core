angular.module('virtoCommerce.storeModule')
    .controller('virtoCommerce.storeModule.storeAdvancedController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.storeModule.stores', 'platformWebApp.common.countries', 'platformWebApp.common.timeZones', function ($scope, bladeNavigationService, fulfillments, countries, timeZones) {
        $scope.saveChanges = function () {
            angular.copy($scope.blade.currentEntity, $scope.blade.origEntity);
            $scope.bladeClose();
        };

        $scope.setForm = function (form) {
            $scope.formScope = form;
        }

        $scope.isValid = function () {
            return $scope.formScope && $scope.formScope.$valid;
        }

        $scope.cancelChanges = function () {
            $scope.bladeClose();
        }

        $scope.blade.refresh = function () {
            
        }

        $scope.blade.headIcon = 'fa-archive';

        $scope.blade.isLoading = false;
        $scope.blade.currentEntity = angular.copy($scope.blade.entity);
        $scope.blade.origEntity = $scope.blade.entity;
        $scope.countries = countries.query();
        $scope.timeZones = timeZones.query();
    }]);
