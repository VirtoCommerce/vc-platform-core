angular.module('virtoCommerce.orderModule').controller('virtoCommerce.orderModule.shipmentDetailController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.settings', 'virtoCommerce.orderModule.order_res_customerOrders', 'virtoCommerce.inventoryModule.fulfillments', 'virtoCommerce.orderModule.statusTranslationService', 'platformWebApp.authService', 'virtoCommerce.shippingModule.shippingMethods',
    function ($scope, bladeNavigationService, dialogService, settings, customerOrders, fulfillments, statusTranslationService, authService, shippingMethods) {
        var blade = $scope.blade;
        blade.isVisiblePrices = authService.checkPermission('order:read_prices');
        blade.shippingMethods = [];

        if (blade.isNew) {
            blade.title = 'orders.blades.shipment-detail.title-new';

            var foundField = _.findWhere(blade.metaFields, { name: 'createdDate' });
            if (foundField) {
                foundField.isReadonly = false;
            }

            customerOrders.getNewShipment({ id: blade.customerOrder.id }, blade.initialize);
        } else {
            blade.title = 'orders.blades.shipment-detail.title';
            blade.titleValues = { number: blade.currentEntity.number };
            blade.subtitle = 'orders.blades.shipment-detail.subtitle';
        }

        blade.currentStore = _.findWhere(blade.parentBlade.stores, { id: blade.customerOrder.storeId });
        blade.realOperationsCollection = blade.customerOrder.shipments;

        shippingMethods.search({ storeId: blade.customerOrder.storeId }, function (data) {
                blade.isLoading = false;
                blade.shippingMethods = data.results;
            }, function (error) {
                bladeNavigationService.setError('Error ' + error.status, blade);
        });

        settings.getValues({ id: 'Shipment.Status' }, translateBladeStatuses);
        blade.openStatusSettingManagement = function () {
            var newBlade = new DictionarySettingDetailBlade('Shipment.Status');
            newBlade.parentRefresh = translateBladeStatuses;
            bladeNavigationService.showBlade(newBlade, blade);
        };

        function translateBladeStatuses(data) {
            blade.statuses = statusTranslationService.translateStatuses(data, 'shipment');
        }

        // load employees
        blade.employees = blade.parentBlade.employees;

        getFulfillmentCenters();
        blade.openFulfillmentCentersList = function () {
            var newBlade = {
                id: 'fulfillmentCenterList',
                controller: 'virtoCommerce.inventoryModule.fulfillmentListController',
                template: 'Modules/$(VirtoCommerce.Inventory)/Scripts/blades/fulfillment-center-list.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, blade);
        };

        blade.customInitialize = function () {
            blade.isLocked = blade.currentEntity.status == 'Send' || blade.currentEntity.isCancelled;
        };

        blade.updateRecalculationFlag = function () {
            blade.isTotalsRecalculationNeeded = blade.origEntity.price != blade.currentEntity.price || blade.origEntity.priceWithTax != blade.currentEntity.priceWithTax;
        }

        function getFulfillmentCenters() {
            fulfillments.search({ take: 100 }, function (response) {
                blade.fulfillmentCenters = response.results;
            });
        }

        $scope.$watch("blade.currentEntity.shippingMethod", function (shippingMethod) {
            if (blade.isNew && shippingMethod) {
                blade.currentEntity.shipmentMethodCode = shippingMethod.code;
            }
          }, true);

    }]);
