angular.module('virtoCommerce.orderModule')
.controller('virtoCommerce.orderModule.paymentDetailController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.settings','virtoCommerce.orderModule.order_res_customerOrders', 'virtoCommerce.orderModule.statusTranslationService',
    function ($scope, bladeNavigationService, dialogService, settings, customerOrders, statusTranslationService) {
        var blade = $scope.blade;

        if (blade.isNew) {
            blade.title = 'orders.blades.payment-detail.title-new';

            var foundField = _.findWhere(blade.metaFields, { name: 'createdDate' });
            if (foundField) {
                foundField.isReadonly = false;
            }

            customerOrders.getNewPayment({ id: blade.customerOrder.id }, blade.initialize);
        } else {
            blade.title = 'orders.blades.payment-detail.title';
            blade.titleValues = { number: blade.currentEntity.number };
            blade.subtitle = 'orders.blades.payment-detail.subtitle';
        }
                
        blade.currentStore = _.findWhere(blade.parentBlade.stores, { id: blade.customerOrder.storeId });
        blade.realOperationsCollection = blade.customerOrder.inPayments;

        settings.getValues({ id: 'PaymentIn.Status' }, translateBladeStatuses);
        blade.openStatusSettingManagement = function () {
            var newBlade = new DictionarySettingDetailBlade('PaymentIn.Status');
            newBlade.parentRefresh = translateBladeStatuses;
            bladeNavigationService.showBlade(newBlade, blade);
        };

        function translateBladeStatuses(data) {
            blade.statuses = statusTranslationService.translateStatuses(data, 'PaymentIn');
        }

        blade.customInitialize = function () {
            blade.isLocked = blade.currentEntity.status == 'Paid' || blade.currentEntity.isCancelled;
        };

        blade.setEntityStatus = function (status) {
            blade.currentEntity.status = status;
            blade.currentEntity.paymentStatus = status;
        };

        blade.updateRecalculationFlag = function () {
            blade.isTotalsRecalculationNeeded = blade.origEntity.price != blade.currentEntity.price || blade.origEntity.priceWithTax != blade.currentEntity.priceWithTax;
        }
    }]);