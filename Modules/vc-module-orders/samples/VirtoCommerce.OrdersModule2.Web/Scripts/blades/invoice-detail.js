angular.module('virtoCommerce.ordersModule2')
.controller('virtoCommerce.ordersModule2.invoiceDetailController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.settings', 'virtoCommerce.customerModule.members',
    function ($scope, bladeNavigationService, dialogService, settings, members) {
        var blade = $scope.blade;

        if (blade.isNew) {
            blade.title = 'New invoice';

            var foundField = _.findWhere(blade.metaFields, { name: 'createdDate' });
            if (foundField) {
                foundField.isReadonly = false;
            }

            blade.initialize({
                operationType: "Invoice",
                status: 'New',
                number: "Inv60826-00000",
                createdDate: new Date(),
                isApproved: true,
                currency: "EUR"
            });
        } else {
            blade.title = 'invoice details';
            blade.subtitle = 'sample';
        }

        blade.currentStore = _.findWhere(blade.parentBlade.stores, { id: blade.customerOrder.storeId });
        blade.realOperationsCollection = blade.customerOrder.invoices;

        blade.statuses = settings.getValues({ id: 'Invoice.Status' });
        blade.openStatusSettingManagement = function () {
            var newBlade = new DictionarySettingDetailBlade('Invoice.Status');
            newBlade.parentRefresh = function (data) {
                blade.statuses = data;
            };
            bladeNavigationService.showBlade(newBlade, blade);
        };

        // load customers
        members.search(
           {
               memberType: 'Contact',
               sort: 'fullName:asc',
               take: 1000
           },
           function (data) {
               blade.contacts = data.results;
           });

        blade.resetCustomerName = function (newVal) {
            blade.currentEntity.customerName = newVal ? newVal.fullName : undefined;
        };
    }]);