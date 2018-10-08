angular.module('virtoCommerce.subscriptionModule')
.controller('virtoCommerce.subscriptionModule.filterDetailController', ['$scope', '$localStorage', 'virtoCommerce.storeModule.stores', 'platformWebApp.settings', 'virtoCommerce.customerModule.members', '$translate',
    function ($scope, $localStorage, storesAPI, settings, members, $translate) {
        var blade = $scope.blade;

        blade.metaFields = [
            {
                name: 'statuses',
                title: "subscription.blades.subscription-detail.labels.status",
                templateUrl: 'statusesSelector.html'
            },
            {
                name: 'storeId',
                title: "orders.blades.customerOrder-detail.labels.store",
                templateUrl: 'storeSelector.html'
            },
            {
                name: 'startDate',
                title: "subscription.blades.filter-detail.labels.from",
                valueType: "DateTime"
            },
            {
                name: 'endDate',
                title: "subscription.blades.filter-detail.labels.to",
                valueType: "DateTime"
            },
            {
                name: 'outerId',
                title: "subscription.blades.filter-detail.labels.outer-id",
                valueType: "ShortText"
            }
        ];

        blade.statuses = settings.getValues({ id: 'Subscription.Status' });
        blade.stores = storesAPI.query();
        
        $scope.saveChanges = function () {
            angular.copy(blade.currentEntity, blade.origEntity);
            if (blade.isNew) {
                $localStorage.subscriptionSearchFilters.push(blade.origEntity);
                $localStorage.subscriptionSearchFilterId = blade.origEntity.id;
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

            blade.title = blade.isNew ? 'subscription.blades.filter-detail.new-title' : data.name;
            blade.subtitle = blade.isNew ? 'subscription.blades.filter-detail.new-subtitle' : 'subscription.blades.filter-detail.subtitle';
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
            $localStorage.subscriptionSearchFilters.splice($localStorage.subscriptionSearchFilters.indexOf(blade.origEntity), 1);
            delete $localStorage.subscriptionSearchFilterId;
            blade.parentBlade.refresh();
            $scope.bladeClose();
        }

        // actions on load
        if (blade.isNew) {
            $translate('subscription.blades.subscription-list.labels.unnamed-filter').then(function (result) {
                initializeBlade({ id: new Date().getTime(), name: result });
            });
        } else {
            initializeBlade(blade.data);
        }
    }]);
