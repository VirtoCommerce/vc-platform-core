angular.module('virtoCommerce.catalogModule')
    .controller('virtoCommerce.catalogModule.categorySelectorController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
        var blade = $scope.blade;

        $scope.selectedCount = (blade.currentEntity.categoryIds || []).length;

        $scope.$watch('blade.currentEntity.categoryIds', function () {
            $scope.selectedCount = (blade.currentEntity.categoryIds || []).length;
        });

        $scope.selectCategory = function () {
            var selection = blade.currentEntity.categoryIds || [];
            var options = {
                allowCheckingCategory: true,
                allowCheckingItem: false,
                selectedItemIds: selection,
                checkItemFn: function (listItem, isSelected) {
                    if (isSelected) {
                        if (!_.find(selection, function (x) { return x === listItem.id; })) {
                            selection.push(listItem.id);
                        }
                    }
                    else {
                        selection = _.reject(selection, function (x) { return x === listItem.id; });
                    }
                }
            };
            var newBlade = {
                id: "CatalogItemsSelect",
                controller: 'virtoCommerce.catalogModule.catalogItemSelectController',
                template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/common/catalog-items-select.tpl.html',
                title: 'catalog.selectors.blades.titles.select-categories',
                options: options,
                breadcrumbs: [],
                toolbarCommands: [
                    {
                        name: "platform.commands.confirm", icon: 'fa fa-check',
                        executeMethod: function (pickingBlade) {
                            blade.currentEntity.categoryIds = _.union(blade.currentEntity.categoryIds, selection);
                            $scope.selectedCount = blade.currentEntity.categoryIds.length;
                            bladeNavigationService.closeBlade(pickingBlade);
                        },
                        canExecuteMethod: function () {
                            return _.any(selection);
                        }
                    },
                    {
                        name: "platform.commands.reset", icon: 'fa fa-undo',
                        executeMethod: function (pickingBlade) {
                            selection = [];
                            blade.currentEntity.categoryIds = [];
                            $scope.selectedCount = blade.currentEntity.categoryIds.length;
                            bladeNavigationService.closeBlade(pickingBlade);
                        },
                        canExecuteMethod: function () {
                            return _.any(selection);
                        }
                    }]
            };
            bladeNavigationService.showBlade(newBlade, blade);
        }
    }]);
