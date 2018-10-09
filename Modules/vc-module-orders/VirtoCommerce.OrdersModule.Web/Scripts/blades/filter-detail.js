angular.module('virtoCommerce.orderModule')
.controller('virtoCommerce.orderModule.filterDetailController', ['$scope', '$localStorage', 'virtoCommerce.orderModule.order_res_stores', 'platformWebApp.settings', 'virtoCommerce.customerModule.members', '$translate', 'virtoCommerce.orderModule.statusTranslationService',
    function ($scope, $localStorage, order_res_stores, settings, members, $translate, statusTranslationService) {
        var blade = $scope.blade;

        blade.metaFields = [
            {
                name: 'statuses',
                title: "orders.blades.customerOrder-detail.labels.status",
                templateUrl: 'statusesSelector.html'
            },
            {
                name: 'storeIds',
                title: "orders.blades.customerOrder-detail.labels.store",
                templateUrl: 'storeSelector.html'
            },
            {
                name: 'startDate',
                title: "orders.blades.filter-detail.labels.from",
                valueType: "DateTime"
            },
            {
                name: 'endDate',
                title: "orders.blades.filter-detail.labels.to",
                valueType: "DateTime"
            },
            {
                name: 'customerId',
                title: "orders.blades.customerOrder-detail.labels.customer",
                templateUrl: 'customerSelector.html'
            },
            {
                name: 'employeeId',
                title: "orders.blades.shipment-detail.labels.employee",
                templateUrl: 'filter-employeeSelector.html'
            }
        ];
        
        function translateBladeStatuses(data) {
            blade.statuses = statusTranslationService.translateStatuses(data, 'customerOrder');
        }
        settings.getValues({ id: 'Order.Status' }, translateBladeStatuses);
        blade.stores = order_res_stores.query();
        // load employees
        members.search(
           {
               memberType: 'Employee',
               sort: 'fullName:asc',
               take: 1000
           },
           function (data) {
               blade.employees = data.results;
           });
        members.search(
           {
               memberType: 'Contact',
               sort: 'fullName:asc',
               take: 1000
           },
           function (data) {
               blade.contacts = data.results;
           });

        $scope.saveChanges = function () {
            angular.copy(blade.currentEntity, blade.origEntity);
            if (blade.isNew) {
                $localStorage.orderSearchFilters.push(blade.origEntity);
                $localStorage.orderSearchFilterId = blade.origEntity.id;
                blade.parentBlade.filter.current = blade.origEntity;
                blade.isNew = false;
            }

            initializeBlade(blade.origEntity);
            blade.parentBlade.filter.criteriaChanged();
            // $scope.bladeClose();
        };

        function initializeBlade(data) {
            blade.currentEntity = angular.copy(data);
            blade.origEntity = data;
            blade.isLoading = false;

            blade.title = blade.isNew ? 'orders.blades.filter-detail.new-title' : data.name;
            blade.subtitle = blade.isNew ? 'orders.blades.filter-detail.new-subtitle' : 'orders.blades.filter-detail.subtitle';
        };

        var formScope;
        $scope.setForm = function (form) { formScope = form; }

        function isDirty() {
            return !angular.equals(blade.currentEntity, blade.origEntity);
        };

        blade.headIcon = 'fa-filter';

        blade.toolbarCommands = [
                {
                    name: "core.commands.apply-filter", icon: 'fa fa-filter',
                    executeMethod: function () {
                        $scope.saveChanges();
                    },
                    canExecuteMethod: function () {
                        return formScope && formScope.$valid;
                    }
                },
                {
                    name: "platform.commands.reset", icon: 'fa fa-undo',
                    executeMethod: function () {
                        angular.copy(blade.origEntity, blade.currentEntity);
                    },
                    canExecuteMethod: isDirty
                },
                {
                    name: "platform.commands.delete", icon: 'fa fa-trash-o',
                    executeMethod: deleteEntry,
                    canExecuteMethod: function () {
                        return !blade.isNew;
                    }
                }];


        function deleteEntry() {
            blade.parentBlade.filter.current = null;
            $localStorage.orderSearchFilters.splice($localStorage.orderSearchFilters.indexOf(blade.origEntity), 1);
            delete $localStorage.orderSearchFilterId;
            blade.parentBlade.refresh();
            $scope.bladeClose();
        }

        // actions on load
        if (blade.isNew) {
            $translate('orders.blades.customerOrder-list.labels.unnamed-filter').then(function (result) {
                initializeBlade({ id: new Date().getTime(), name: result });
            });
        } else {
            initializeBlade(blade.data);
        }
    }]);
