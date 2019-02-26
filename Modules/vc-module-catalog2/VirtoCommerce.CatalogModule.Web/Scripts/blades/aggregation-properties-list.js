angular.module('virtoCommerce.catalogModule')
    .controller('virtoCommerce.catalogModule.aggregationPropertiesController', ['$scope', 'platformWebApp.dialogService', 'platformWebApp.bladeNavigationService', 'virtoCommerce.catalogModule.aggregationProperties', function ($scope, dialogService, bladeNavigationService, aggregationProperties) {
    var blade = $scope.blade;
    blade.updatePermission = 'store:update';

    function initializeBlade() {
        aggregationProperties.getProperties({ storeId: blade.storeId }, function (results) {
            blade.currentEntities = angular.copy(results);
            blade.origEntity = results;

            blade.selectedEntities = _.where(blade.currentEntities, { isSelected: true });
            blade.origSelected = angular.copy(blade.selectedEntities);

            blade.isLoading = false;
        }, function (error) {
            bladeNavigationService.setError('Error ' + error.status, blade);
        });
    }

    blade.edit = function (node) {
        var newBlade = {
            id: "aggregationPropertyDetails",
            controller: 'virtoCommerce.catalogModule.aggregationPropertyDetailsController',
            template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/aggregation-properties-details.tpl.html',
            title: node.name,
            storeId: blade.storeId,
            property: node
        };
        bladeNavigationService.showBlade(newBlade, blade);
    }

    blade.select = function (node) {
        node.isSelected = true;
        blade.selectedEntities.push(node);
    };

    blade.unselect = function (node) {
        node.isSelected = false;
        blade.selectedEntities.splice(blade.selectedEntities.indexOf(node), 1);
    };

    function isDirty() {
        return !angular.equals(blade.selectedEntities, blade.origSelected) && blade.hasUpdatePermission();
    }
    
    blade.onClose = function (closeCallback) {
        bladeNavigationService.showConfirmationIfNeeded(isDirty(), true, blade, $scope.saveChanges, closeCallback, "Save changes", "The properties selection has been modified. Do you want to confirm changes?");
    };

    $scope.saveChanges = function () {
        blade.isLoading = true;

        aggregationProperties.setProperties({ storeId: blade.storeId }, blade.selectedEntities, function (data) {
            angular.copy(blade.currentEntities, blade.origEntity);
            angular.copy(blade.selectedEntities, blade.origSelected);
            // $scope.bladeClose();
            blade.isLoading = false;
        }, function (error) {
            bladeNavigationService.setError('Error: ' + error.status, blade);
        });
    };

    blade.toolbarCommands = [
        {
            name: "Save", icon: 'fa fa-save',
            executeMethod: $scope.saveChanges,
            canExecuteMethod: isDirty
        },
        {
            name: "Reset", icon: 'fa fa-undo',
            executeMethod: function () {
                angular.copy(blade.origEntity, blade.currentEntities);
                blade.selectedEntities = _.where(blade.currentEntities, { isSelected: true });
                angular.copy(blade.selectedEntities, blade.origSelected);
            },
            canExecuteMethod: isDirty
            // permission: 'catalog:update'
        }
    ];

    $scope.sortableOptions = {
        axis: 'y',
        cursor: "move"
    };

    blade.headIcon = 'fa-gear';
    initializeBlade();
}]);
