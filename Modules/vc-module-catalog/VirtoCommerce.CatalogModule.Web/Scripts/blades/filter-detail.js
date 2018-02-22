angular.module('virtoCommerce.catalogModule')
.controller('virtoCommerce.catalogModule.filterDetailController', ['$scope', '$localStorage', '$translate',
    function ($scope, $localStorage, $translate) {
        var blade = $scope.blade;

        $scope.saveChanges = function () {
            blade.currentEntity.lastUpdateTime = new Date().getTime();
            angular.copy(blade.currentEntity, blade.origEntity);
            if (blade.isNew) {
                $localStorage.catalogSearchFilters.splice(0, 0, blade.origEntity);
                $localStorage.catalogSearchFilterId = blade.origEntity.id;
                blade.parentBlade.filter.current = blade.origEntity;
                blade.isNew = false;
            }

            initializeBlade(blade.origEntity);
            blade.parentBlade.filter.change(true);
        };

        function initializeBlade(data) {
            blade.currentEntity = angular.copy(data);
            blade.origEntity = data;
            blade.isLoading = false;

            blade.title = blade.isNew ? 'catalog.blades.filter-detail.new-title' : data.name;
            blade.subtitle = blade.isNew ? 'catalog.blades.filter-detail.new-subtitle' : 'catalog.blades.filter-detail.subtitle';
        }

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
            $localStorage.catalogSearchFilters.splice($localStorage.catalogSearchFilters.indexOf(blade.origEntity), 1);
            delete $localStorage.catalogSearchFilterId;
            blade.parentBlade.filter.change();
        }

        // actions on load        
        if (blade.isNew) {
            $translate('catalog.blades.categories-items-list.labels.unnamed-filter').then(function (result) {
                initializeBlade({ id: new Date().getTime(), name: result });
            });
        } else {
            initializeBlade(blade.data);
        }
    }]);
