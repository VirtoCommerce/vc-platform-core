angular.module('virtoCommerce.inventoryModule')
    .controller('virtoCommerce.inventoryModule.fulfillmentAddressesWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.metaFormsService', function ($scope, bladeNavigationService, metaFormsService) {
        var blade = $scope.widget.blade;

        $scope.address = blade.currentEntity.address;
        var addressMetaFields = [
             {
                name: 'countryCode',
                templateUrl: 'countrySelector.html',
                priority: 4
            }, {
                name: 'regionName',
                title: 'inventory.widgets.fulfillmentWidget.address-detail.region',
                valueType: 'ShortText',
                priority: 5
            }, {
                name: 'city',
                title: 'inventory.widgets.fulfillmentWidget.address-detail.city',
                valueType: 'ShortText',
                 priority: 6
            }, {
                name: 'line1',
                title: 'inventory.widgets.fulfillmentWidget.address-detail.address1',
                valueType: 'ShortText',
                priority: 7
            },
            {
                name: 'postalCode',
                title: 'inventory.widgets.fulfillmentWidget.address-detail.zip-code',
                valueType: 'ShortText',
                priority: 9
            }, {
                name: 'email',
                title: 'inventory.widgets.fulfillmentWidget.address-detail.email',
                valueType: 'Email',
                priority: 10
            }, {
                name: 'phone',
                title: 'inventory.widgets.fulfillmentWidget.address-detail.phone',
                valueType: 'ShortText',
                priority: 11
            }];

        var metaFields = metaFormsService.getMetaFields('fulfillmentAddressesWidget');
        if (metaFields && metaFields.length) {
            addressMetaFields = _.sortBy(addressMetaFields.concat(metaFields), 'priority');
        }

        $scope.openBlade = function () {
            var newBlade = {
                id: 'coreAddressDetail',
                currentEntity: $scope.address ? $scope.address : { isNew: true  },
                metaFields : addressMetaFields,
                title: blade.title,
                subtitle: 'inventory.widgets.fulfillmentWidget.address-detail.subtitle',
                controller: 'virtoCommerce.coreModule.common.coreAddressDetailController',
                confirmChangesFn: function (address) {

                    blade.currentEntity.address = address;
                    address.isEmpty = false;
                    if (blade.confirmChangesFn) {
                        blade.confirmChangesFn(address);
                    }
                },
                deleteFn: function (address) {
                    blade.currentEntity.address = {};
                },
                template: 'Modules/$(VirtoCommerce.Core)/Scripts/common/blades/address-detail.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, $scope.blade);
        };

        $scope.$watch('blade.currentEntity', function (data) {
            if (data && data.address) {
                $scope.address = data.address;
            }
        });


    }]);
