angular.module('virtoCommerce.catalogModule')
.controller('virtoCommerce.catalogModule.itemPropertyDetailController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'virtoCommerce.catalogModule.valueTypes', function ($scope, bladeNavigationService, dialogService, valueTypes) {
    var blade = $scope.blade;
    blade.availableValueTypes = valueTypes.get();
    $scope.isValid = false;
  
    blade.title = "catalog.blades.item-property-detail.title";
    blade.subtitle = "catalog.blades.item-property-detail.subtitle";

    $scope.$watch("blade.currentEntity", function () {
    	$scope.isValid = $scope.formScope && $scope.formScope.$valid;
    }, true);

    function initialize(data) {
        if (data.valueType === 'Number' && data.dictionaryValues) {
            _.forEach(data.dictionaryValues, function (entry) {
                entry.value = parseFloat(entry.value);
            });
        }
        blade.currentEntity = angular.copy(data);
        blade.isLoading = false;
    };

   
    $scope.saveChanges = function () {
        angular.copy(blade.currentEntity, blade.origEntity);
        if (blade.isNew && blade.properties) {
            blade.properties.push(blade.origEntity);
        }
        $scope.bladeClose();
    };

    function removeProperty(prop) {
        var dialog = {
            id: "confirmDelete",
            title: "platform.dialogs.delete.title",
            message: 'catalog.dialogs.item-property-delete.message',
            messageValues: { name: prop.name },
            callback: function (remove) {
                if (remove) {
                	var idx = blade.properties.indexOf(blade.origEntity);
                	blade.properties.splice(idx, 1);
                    $scope.bladeClose();
                }
            }
        }
        dialogService.showConfirmationDialog(dialog);
    }

    $scope.setForm = function (form) { $scope.formScope = form; };

    if (!blade.isNew) {
        blade.headIcon = 'fa-gear';
        blade.toolbarCommands = [           
            {
                name: "platform.commands.delete", icon: 'fa fa-trash-o',
                executeMethod: function () {
                    removeProperty(blade.origEntity);
                },
                canExecuteMethod: function () { return true; }
            }
        ];
    }
    // actions on load    
    initialize(blade.origEntity);
}]);
