angular.module('virtoCommerce.pricingModule')
.controller('virtoCommerce.pricingModule.pricesListController', ['$scope', 'virtoCommerce.pricingModule.prices', 'platformWebApp.objCompareService', 'platformWebApp.bladeNavigationService', 'platformWebApp.uiGridHelper', 'virtoCommerce.pricingModule.priceValidatorsService', 'platformWebApp.ui-grid.extension', function ($scope, prices, objCompareService, bladeNavigationService, uiGridHelper, priceValidatorsService, gridOptionExtension) {
    $scope.uiGridConstants = uiGridHelper.uiGridConstants;
    var blade = $scope.blade;
    blade.updatePermission = 'pricing:update';

    blade.refresh = function () {
        blade.data.productId = blade.itemId;
        if (!blade.data.prices) {
            blade.data.prices = [];
        }

        //if (!_.any(blade.data.prices)) {
        //    addNewPrice(blade.data.prices);
        //}

        blade.currentEntities = angular.copy(blade.data.prices);
        blade.origEntity = blade.data.prices;
        blade.isLoading = false;
        priceValidatorsService.setAllPrices(blade.currentEntities);
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

    $scope.cancelChanges = function () {
        $scope.bladeClose();
    };

    $scope.isValid = function () {
        return $scope.formScope && $scope.formScope.$valid &&
             _.all(blade.currentEntities, $scope.isListPriceValid) &&
             _.all(blade.currentEntities, $scope.isSalePriceValid) &&
             _.all(blade.currentEntities, $scope.isUniqueQty) &&
            (blade.currentEntities.length == 0 || _.some(blade.currentEntities, function (x) { return x.minQuantity == 1; }));
    }

    $scope.saveChanges = function () {
        blade.isLoading = true;

        angular.copy(blade.currentEntities, blade.origEntity);
        if (_.any(blade.currentEntities)) {
            prices.update({ id: blade.itemId }, blade.data, function (data) {
                // blade.parentBlade.refresh();
                $scope.bladeClose();
            },
            function (error) { bladeNavigationService.setError('Error ' + error.status, $scope.blade); });
        }
        else {
            prices.remove({ priceListId: blade.priceListId, productIds: [blade.itemId] }, function () {
                $scope.bladeClose();
                blade.parentBlade.refresh();
            },
            function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
        }
    };

    $scope.delete = function (listItem) {
        if (listItem) {
            blade.currentEntities.splice(blade.currentEntities.indexOf(listItem), 1);
        }
    };

    $scope.setForm = function (form) { $scope.formScope = form; }

    blade.toolbarCommands = [
        {
            name: "platform.commands.save", icon: 'fa fa-save',
            executeMethod: $scope.saveChanges,
            canExecuteMethod: canSave,
            permission: blade.updatePermission
        },
        {
            name: "platform.commands.reset", icon: 'fa fa-undo',
            executeMethod: function () {
                angular.copy(blade.origEntity, blade.currentEntities);
            },
            canExecuteMethod: isDirty,
            permission: blade.updatePermission
        },
        {
            name: "platform.commands.add", icon: 'fa fa-plus',
            executeMethod: function () { addNewPrice(blade.currentEntities); },
            canExecuteMethod: function () { return true; },
            permission: blade.updatePermission
        },
        {
            name: "platform.commands.delete", icon: 'fa fa-trash-o',
            executeMethod: function () {
                var selection = $scope.gridApi.selection.getSelectedRows();
                angular.forEach(selection, function (listItem) {
                    blade.currentEntities.splice(blade.currentEntities.indexOf(listItem), 1);
                });
            },
            canExecuteMethod: function () {
                return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
            },
            permission: blade.updatePermission
        }
    ];

    function addNewPrice(targetList) {
        var newEntity = { productId: blade.itemId, list: '', minQuantity: 1, currency: blade.currency, priceListId: blade.priceListId };
        targetList.push(newEntity);
        $scope.validateGridData();
    }

    $scope.isListPriceValid = priceValidatorsService.isListPriceValid;
    $scope.isSalePriceValid = priceValidatorsService.isSalePriceValid;
    $scope.isUniqueQty = priceValidatorsService.isUniqueQty;

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

    // actions on load
    blade.refresh();
}])

.factory('virtoCommerce.pricingModule.priceValidatorsService', [function () {
    var allPrices = {};
    return {
        setAllPrices: function (data) {
            allPrices = data;
        },
        isListPriceValid: function (data) {
            return data.list >= 0;
        },
        isSalePriceValid: function (data) {
            return _.isUndefined(data.sale) || data.list >= data.sale;
        },
        isUniqueQty: function (data) {
            return Math.round(data.minQuantity) > 0 && _.all(allPrices, function (x) { return x === data || Math.round(x.minQuantity) !== Math.round(data.minQuantity) });
        },
        isUniqueQtyForPricelist: function (data) {
            return _.filter(allPrices, function (price) { return price.pricelistId == data.pricelistId && price.minQuantity == data.minQuantity }).length == 1;
        }
    };
}])

.run(
  ['platformWebApp.ui-grid.extension', 'virtoCommerce.pricingModule.priceValidatorsService', 'uiGridValidateService', function (gridOptionExtension, priceValidatorsService, uiGridValidateService) {
     
      uiGridValidateService.setValidator('listValidator', function (argument) {
          return function (oldValue, newValue, rowEntity, colDef) {
              // We should not test for existence here
              return _.isUndefined(newValue) || priceValidatorsService.isListPriceValid(rowEntity);
          };
      }, function (argument) { return 'List price is invalid '; });

      uiGridValidateService.setValidator('saleValidator', function (argument) {
          return function (oldValue, newValue, rowEntity, colDef) {
              return priceValidatorsService.isSalePriceValid(rowEntity);
          };
      }, function (argument) { return 'Sale price should not exceed List price'; });

      uiGridValidateService.setValidator('minQuantityValidator', function () {
          return function (oldValue, newValue, rowEntity, colDef) {
              return priceValidatorsService.isUniqueQty(rowEntity);
          };
      }, function () { return 'Quantity value should be unique'; });

      uiGridValidateService.setValidator('minQuantityForPricelistValidator', function () {
          return function (oldValue, newValue, rowEntity, colDef) {
              return priceValidatorsService.isUniqueQtyForPricelist(rowEntity);
          };
      }, function () { return 'Quantity value should be unique'; });
  }]);
