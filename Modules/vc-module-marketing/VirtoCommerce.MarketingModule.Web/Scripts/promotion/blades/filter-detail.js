angular.module('virtoCommerce.marketingModule')
.controller('virtoCommerce.marketingModule.filterDetailController', ['$scope', '$localStorage', '$translate', 'virtoCommerce.storeModule.stores',
    function ($scope, $localStorage, $translate, stores) {
        var blade = $scope.blade;

        function initializeBlade(data) {
            blade.currentEntity = angular.copy(data);
            blade.origEntity = data;
            blade.isLoading = false;

            blade.title = blade.isNew ? 'marketing.blades.filter-detail.new-title' : data.name;
            blade.subtitle = blade.isNew ? 'marketing.blades.filter-detail.new-subtitle' : 'marketing.blades.filter-detail.subtitle';
        }

        blade.metaFields = [
            {
                name: 'onlyActive',
                title: "marketing.blades.filter-detail.labels.active",
                valueType: "Boolean"
            }, {
                name: 'store',
                title: "marketing.blades.promotion-detail.labels.store",
                templateUrl: 'storeSelector.html'
            //}, {
            //    name: 'startDate',
            //    title: "marketing.blades.promotion-detail.labels.start-date",
            //    valueType: "DateTime"
            //}, {
            //    name: 'endDate',
            //    title: "marketing.blades.promotion-detail.labels.expiration-date",
            //    valueType: "DateTime"
            }
        ];

        $scope.saveChanges = function () {
            angular.copy(blade.currentEntity, blade.origEntity);
            if (blade.isNew) {
                $localStorage.promotionSearchFilters.push(blade.origEntity);
                $localStorage.promotionSearchFilterId = blade.origEntity.id;
                blade.parentBlade.filter.current = blade.origEntity;
                blade.isNew = false;
            }

            initializeBlade(blade.origEntity);
            blade.parentBlade.filter.criteriaChanged();
        };

        var formScope;
        $scope.setForm = function (form) { formScope = form; };

        function isDirty() {
            return !angular.equals(blade.currentEntity, blade.origEntity);
        }

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
            $localStorage.promotionSearchFilters.splice($localStorage.promotionSearchFilters.indexOf(blade.origEntity), 1);
            delete $localStorage.promotionSearchFilterId;
            blade.parentBlade.refresh();
            $scope.bladeClose();
        }

        blade.stores = stores.query();

        // actions on load
        if (blade.isNew) {
            $translate('marketing.blades.filter-detail.unnamed-filter').then(function (result) {
                initializeBlade({ id: new Date().getTime(), name: result });
            });
        } else {
            initializeBlade(blade.data);
        }
    }]);
