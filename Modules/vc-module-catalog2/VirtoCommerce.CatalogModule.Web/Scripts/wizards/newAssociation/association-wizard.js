angular.module('virtoCommerce.catalogModule')
.controller('virtoCommerce.catalogModule.associationWizardController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.settings', 'virtoCommerce.catalogModule.items', function ($scope, bladeNavigationService, settings, items) {
    var blade = $scope.blade;
    blade.title = "catalog.wizards.association.title";
    blade.isLoading = false;

    $scope.selectedCount = function (type) {
        return _.where(blade.selection, { type: type }).length;
    };

    function initialize(item) {
        $scope.associationGroups = settings.getValues({ id: 'Catalog.AssociationGroups' }, function (data) {
            if (data && data.length > 0) {
                blade.groupName = data[0];
            }
        });
        blade.selection = _.map(blade.item.associations, function (x) {
            return { id: x.associatedObjectId, type: x.associatedObjectType, association: x }
        });
    };
    $scope.isValid = false;
    $scope.$watch("blade.selection", function () {
        $scope.isValid = blade.groupName && blade.selection.length > 0;
    }, true);


    $scope.saveChanges = function () {
        blade.item.associations = _.map(blade.selection, function (x) {
            var retVal = x.association;
            if (!retVal) {
                retVal = {
                    type: blade.groupName,
                    associatedObjectType: x.type,
                    associatedObjectId: x.id,
                    associatedObjectName: x.name,
                    associatedObjectImg: x.imageUrl,
                    quantity: blade.quantity
                };
                if (_.any(blade.tags)) {
                    retVal.tags = _.map(blade.tags, function (x) {
                        return x.value;
                    });
                }
            }
            return retVal;
        });
        if (angular.isFunction(blade.onSaveChanges)) {
            blade.onSaveChanges();                
        }
        $scope.bladeClose();
    };


    $scope.openBlade = function () {
        var selection = [];
        var options = {
            allowCheckingCategory: true,
            selectedItemIds: [],
            checkItemFn: function (listItem, isSelected) {
                if (isSelected) {
                    if (!_.find(selection, function (x) { return x.id == listItem.id; })) {
                        selection.push(listItem);
                    }
                }
                else {
                    selection = _.reject(selection, function (x) { return x.id == listItem.id; });
                }
            }
        };
        var newBlade = {
            id: "CatalogItemsSelect",
            title: "catalog.blades.catalog-items-select.title-association",
            subtitle: 'catalog.blades.catalog-items-select.subtitle-association',
            controller: 'virtoCommerce.catalogModule.catalogItemSelectController',
            template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/common/catalog-items-select.tpl.html',
            options: options,
            breadcrumbs: [],
            toolbarCommands: [
              {
                  name: "platform.commands.confirm", icon: 'fa fa-check',
                  executeMethod: function (pickingBlade) {
                      blade.selection = _.uniq(_.union(blade.selection, selection), function (x) {
                          return [x.type, x.id, x.association ? x.association.type : blade.groupName].join();
                      });
                      bladeNavigationService.closeBlade(pickingBlade);
                  },
                  canExecuteMethod: function () {
                      return _.any(selection);
                  }
              }]
        };
        bladeNavigationService.showBlade(newBlade, blade);
    }


    $scope.openDictionarySettingManagement = function () {
        var newBlade = {
            id: 'settingDetailChild',
            isApiSave: true,
            currentEntityId: 'Catalog.AssociationGroups',
            parentRefresh: function (data) { $scope.associationGroups = data; },
            controller: 'platformWebApp.settingDictionaryController',
            template: '$(Platform)/Scripts/app/settings/blades/setting-dictionary.tpl.html'
        };
        bladeNavigationService.showBlade(newBlade, blade);
    };

    initialize(blade.item);

}]);