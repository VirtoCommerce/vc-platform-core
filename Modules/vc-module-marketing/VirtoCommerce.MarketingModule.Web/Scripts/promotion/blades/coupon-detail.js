angular.module('virtoCommerce.marketingModule')
.controller('virtoCommerce.marketingModule.couponDetailController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.marketingModule.promotions', function ($scope, bladeNavigationService, promotionsApi) {
    var blade = $scope.blade;
    blade.isLoading = false;

    blade.refresh = function (parentRefresh) {
        if (!blade.isNew) {
            promotionsApi.getCoupon({
                id: blade.currentEntity.id
            }, function (response) {
                blade.currentEntity = response;
                blade.parentBlade.refresh();
            }, function (error) {
                bladeNavigationService.setError('Error ' + error.status, blade);
            });
        }
    }

    function isDirty() {
        return !angular.equals(blade.currentEntity, blade.originalEntity);
    }

    $scope.setForm = function (form) {
        $scope.formCoupon = form;
    }

    $scope.datepickers = {
        expirationDate: false
    }

    $scope.openCalendar = function ($event, calendar) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.datepickers[calendar] = true;
    }

    $scope.format = 'shortDate';

    $scope.isValid = function () {
        return isDirty()
            && $scope.formCoupon
            && $scope.formCoupon.$valid;
    };

    $scope.saveChanges = function () {
        blade.isLoading = true;
        promotionsApi.saveCoupon([
            blade.currentEntity
        ], function (response) {
            blade.isLoading = false;
            blade.parentBlade.refresh();
            bladeNavigationService.closeBlade(blade);
        }, function (error) {
            bladeNavigationService.setError('Error ' + error.status, blade);
        });
    }

    blade.toolbarCommands = [{
        name: "platform.commands.save",
        icon: 'fa fa-save',
        executeMethod: $scope.saveChanges,
        canExecuteMethod: $scope.isValid
    }];
}]);