angular.module('virtoCommerce.pricingModule')
.controller('virtoCommerce.pricingModule.itemPriceListController', ['$scope', 'platformWebApp.bladeNavigationService', 'uiGridConstants', 'virtoCommerce.pricingModule.prices', 'virtoCommerce.catalogModule.catalogs', 'platformWebApp.ui-grid.extension', 'platformWebApp.objCompareService', 'virtoCommerce.pricingModule.priceValidatorsService', 'platformWebApp.dialogService',
    function ($scope, bladeNavigationService, uiGridConstants, prices, catalogs, gridOptionExtension, objCompareService, priceValidatorsService, dialogService) {
        $scope.uiGridConstants = uiGridConstants;
        var blade = $scope.blade;
        blade.updatePermission = 'pricing:update';

        blade.refresh = function () {
            blade.isLoading = true;
            prices.getProductPricelists({ id: blade.itemId }, function (pricelists) {
                //Loading catalogs for assignments because they do not contains them
                //Need to display name of catalog in assignments grid
                catalogs.getCatalogs(function (catalogsList) {
                    $scope.catalogsList = catalogsList;

                    blade.origEntity = [];

                    //Collect all available pricelists
                    blade.pricelistList = _.map(pricelists, function (pricelist) {
                        return {
                            id: pricelist.id,
                            code: pricelist.name,
                            currency: pricelist.currency,
                            assignments: pricelist.assignments,
                            displayName: pricelist.name + ' - ' + pricelist.currency
                        };
                    });
                    blade.selectedPricelist = _.first(blade.pricelistList);

                    var pricelistsWithPrices = _.filter(pricelists, function (pricelist) { return pricelist.prices.length > 0; });
                    _.each(pricelistsWithPrices, function (pricelistWithPrices) {
                        var priceListData = {
                            name: pricelistWithPrices.name,
                            currency: pricelistWithPrices.currency
                        };

                        var catalogsId = _.pluck(pricelistWithPrices.assignments, 'catalogId');
                        var catalogsName = _.map(catalogsId, function (catalogId) {
                            return _.findWhere($scope.catalogsList, { id: catalogId }).name;
                        });
                        priceListData.catalog = catalogsName.join(', ');

                        _.each(pricelistWithPrices.prices, function (price) {
                            var priceData = angular.copy(priceListData);
                            priceData = angular.extend(price, priceData);
                            blade.origEntity.push(priceData);
                        });
                    });

                    blade.currentEntities = angular.copy(blade.origEntity);
                    priceValidatorsService.setAllPrices(blade.currentEntities);
                    blade.isLoading = false;
                });
            });
        }

        $scope.createNewPricelist = function () {
            var newBlade = {
                id: 'pricingList',
                controller: 'virtoCommerce.pricingModule.pricelistListController',
                template: 'Modules/$(VirtoCommerce.Pricing)/Scripts/blades/pricelist-list.tpl.html',
                title: 'pricing.blades.pricing-main.menu.pricelist-list.title',
                parentRefresh: blade.refresh
            };

            bladeNavigationService.showBlade(newBlade, blade);
        };

        $scope.selectPricelist = function (entity) {
            var newBlade = {
                id: 'listItemChild',
                currentEntityId: entity.pricelistId,
                title: entity.name,
                controller: 'virtoCommerce.pricingModule.pricelistDetailController',
                template: 'Modules/$(VirtoCommerce.Pricing)/Scripts/blades/pricelist-detail.tpl.html'
            };

            bladeNavigationService.showBlade(newBlade, blade);
        };

        blade.onClose = function (closeCallback) {
            bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "pricing.dialogs.prices-save.title", "pricing.dialogs.prices-save.message");
        };

        function isDirty() {
            return blade.currentEntities && !objCompareService.equal(blade.origEntity, blade.currentEntities) && blade.hasUpdatePermission()
        }

        function canSave() {
            return isDirty() && $scope.isValid();
        }

        $scope.isValid = function () {
            return $scope.formScope && $scope.formScope.$valid &&
                 _.all(blade.currentEntities, $scope.isListPriceValid) &&
                 _.all(blade.currentEntities, $scope.isSalePriceValid) &&
                 _.all(blade.currentEntities, $scope.isUniqueQtyForPricelist) &&
                (blade.currentEntities.length == 0 || _.some(blade.currentEntities, function (x) { return x.minQuantity == 1; }));
        }

        $scope.saveChanges = function () {
            blade.isLoading = true;

            angular.copy(blade.currentEntities, blade.origEntity);
            if (_.any(blade.currentEntities)) {
                var productPrices =  {
                    productId: blade.itemId,
                    product: blade.item,
                    prices: blade.currentEntities
                };
                prices.update({ id: blade.itemId }, productPrices, function (data) {
                    blade.refresh();
                },
                function (error) { bladeNavigationService.setError('Error ' + error.status, $scope.blade); });
            }
        };

        $scope.setForm = function (form) { $scope.formScope = form; }

        blade.toolbarCommands = [
            {
                name: "platform.commands.save",
                icon: 'fa fa-save',
                executeMethod: $scope.saveChanges,
                canExecuteMethod: canSave,
                permission: blade.updatePermission
            },
            {
                name: "platform.commands.delete",
                icon: 'fa fa-trash-o',
                executeMethod: function () {
                    var selection = $scope.gridApi.selection.getSelectedRows();
                    var ids = _.map(selection, function (item) { return item.id; });

                    var dialog = {
                        id: "confirmDeleteItem",
                        title: "pricing.dialogs.item-prices-delete.title",
                        message: "pricing.dialogs.item-prices-delete.message",
                        callback: function (remove) {
                            if (remove) {
                                prices.removePrice({ priceIds: ids }, function () {
                                    //blade.refresh();
                                    angular.forEach(selection, function (listItem) {
                                        blade.currentEntities.splice(blade.currentEntities.indexOf(listItem), 1);
                                    });
                                }, function (error) {
                                    bladeNavigationService.setError('Error ' + error.status, blade);
                                });
                            }
                        }
                    }
                    dialogService.showConfirmationDialog(dialog);
                },
                canExecuteMethod: function () {
                    return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
                },
                permission: 'pricing:delete'
            },
            {
                name: "platform.commands.refresh",
                icon: 'fa fa-refresh',
                executeMethod: blade.refresh,
                canExecuteMethod: function () { return true; }
            }
        ];

        blade.addNewPrice = function (targetPricelist) {
            //populate prices data for correct work of validation service
            priceValidatorsService.setAllPrices(blade.currentEntities);

            var catalogsId = _.pluck(targetPricelist.assignments, 'catalogId');
            var catalogsName = _.map(catalogsId, function (catalogId) {
                return _.findWhere($scope.catalogsList, { id: catalogId }).name;
            });

            var newPrice = {
                productId: blade.itemId,
                list: '',
                minQuantity: 1,
                currency: targetPricelist.currency,
                pricelistId: targetPricelist.id,
                name: targetPricelist.code,
                catalog: catalogsName.join(', ')
            };
            blade.currentEntities.push(newPrice);
            $scope.validateGridData();
        }

        $scope.isListPriceValid = priceValidatorsService.isListPriceValid;
        $scope.isSalePriceValid = priceValidatorsService.isSalePriceValid;
        $scope.isUniqueQtyForPricelist = priceValidatorsService.isUniqueQtyForPricelist;

        // ui-grid
        $scope.setGridOptions = function (gridId, gridOptions) {
            gridOptions.onRegisterApi = function (gridApi) {
                $scope.gridApi = gridApi;

                gridApi.edit.on.afterCellEdit($scope, function () {
                    //to process validation for all rows in grid.
                    //e.g. if we have two rows with the same count of min qty, both of this rows will be marked as error.
                    //when we change data to valid in one row, another one should became valid too.
                    //more info about ui-grid validation: https://github.com/angular-ui/ui-grid/issues/4152
                    $scope.validateGridData();
                });

                $scope.validateGridData();
            };

            $scope.gridOptions = gridOptions;
            gridOptionExtension.tryExtendGridOptions(gridId, gridOptions);
            return gridOptions;
        };

        $scope.validateGridData = function () {
            if ($scope.gridApi) {
                angular.forEach(blade.currentEntities, function (rowEntity) {
                    angular.forEach($scope.gridOptions.columnDefs, function (colDef) {
                        $scope.gridApi.grid.validate.runValidators(rowEntity, colDef, rowEntity[colDef.name], undefined, $scope.gridApi.grid)
                    });
                });
            }
        };

        blade.refresh();
}]);
