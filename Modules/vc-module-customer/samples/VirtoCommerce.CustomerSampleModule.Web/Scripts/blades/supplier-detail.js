angular.module('virtoCommerce.customerSampleModule')
    .controller('virtoCommerce.customerSampleModule.supplierDetailController',  ['$scope', function ($scope) {
        var blade = $scope.blade;
        
        if (blade.isNew) {
            blade.title = 'New Supplier';
            blade.currentEntity = angular.extend({
                reviews: []
            }, blade.currentEntity);

            blade.fillDynamicProperties();
        } else {
            blade.subtitle = 'Supplier details';
        }

        // base function override (optional)
        blade.customInitialize = function () {
            if (!blade.isNew) {
                blade.title = blade.currentEntity.name + '\'s details';
            }
        };
    }]);