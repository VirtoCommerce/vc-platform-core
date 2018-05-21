angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.filterJournalDetailController', ['$scope', '$localStorage', '$translate',
    function ($scope, $localStorage, $translate) {
        var blade = $scope.blade;
        var formScope;
        $scope.setForm = function (form) { formScope = form; };
        
        function initializeBlade(data) {
            blade.currentEntity = angular.copy(data);
            blade.origEntity = data;
            blade.isLoading = false;

            blade.title = blade.isNew ? 'notifications.blades.filter-detail.new-title' : data.name;
            blade.subtitle = blade.isNew ? 'notifications.blades.filter-detail.new-subtitle' : 'notifications.blades.filter-detail.subtitle';
        }
        
        $scope.saveChanges = function () {
            blade.currentEntity.lastUpdateTime = new Date().getTime();
            angular.copy(blade.currentEntity, blade.origEntity);
            if (blade.isNew) {
                $localStorage.notificationsJournalSearchFilters.splice(0, 0, blade.origEntity);
                $localStorage.notificationsJournalSearchFilterId = blade.origEntity.id;
                blade.parentBlade.filter.current = blade.origEntity;
                blade.isNew = false;
            }

            initializeBlade(blade.origEntity);
            blade.parentBlade.filter.change(true);
        };

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
            $localStorage.notificationsJournalSearchFilters.splice($localStorage.notificationsJournalSearchFilters.indexOf(blade.origEntity), 1);
            delete $localStorage.notificationsJournalSearchFilterId;
            blade.parentBlade.filter.change();
        }

        // actions on load        
        if (blade.isNew) {
            $translate('notifications.blades.notifications-journal.labels.unnamed-filter').then(function (result) {
                initializeBlade({ id: new Date().getTime(), name: result });
            });
        } else {
            initializeBlade(blade.data);
        }
    }]);
