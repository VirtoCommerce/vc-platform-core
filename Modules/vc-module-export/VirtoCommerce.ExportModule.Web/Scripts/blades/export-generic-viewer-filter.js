angular.module('virtoCommerce.exportModule')
.controller('virtoCommerce.exportModule.exportGenericViewerFilterController', ['$scope', '$localStorage', 'platformWebApp.metaFormsService', '$translate', 
    function ($scope, $localStorage, metaFormsService, $translate) {
        var blade = $scope.blade;
        blade.exportTypeName = blade.exportTypeName || "NotSpecified";

        blade.metaFields = metaFormsService.getMetaFields(blade.metafieldsId) || [];

        $scope.apply = function (needSave) {
            if (blade.onBeforeApply) {
                blade.onBeforeApply();
            }

            angular.copy(blade.currentEntity, blade.origEntity);

            if (needSave) {
                $localStorage.exportSearchFilterIds[blade.exportTypeName] = blade.origEntity.id;

                var savedFilterIndex = _.findIndex($localStorage.exportSearchFilters[blade.exportTypeName], { id: blade.origEntity.id });

                if (savedFilterIndex !== -1) {
                    $localStorage.exportSearchFilters[blade.exportTypeName][savedFilterIndex] = blade.origEntity;
                } else {
                    $localStorage.exportSearchFilters[blade.exportTypeName].push(blade.origEntity);
                }
            } 

            if (blade.isNew) {
                if (!needSave) {
                    $localStorage.exportSearchFilterIds[blade.exportTypeName] = undefined;
                }

                blade.parentBlade.filter.current = blade.origEntity;
                blade.isNew = false;
            }

            initializeBlade(blade.origEntity);
            blade.parentBlade.filter.current.objectIds = []; // Reset object ids
            blade.parentBlade.filter.criteriaChanged();
            // $scope.bladeClose();
        };

        function initializeBlade(data) {
            blade.currentEntity = angular.copy(data);
            blade.origEntity = data;
            blade.isLoading = false;

            blade.title = blade.isNew ? 'export.blades.generic-filter.new-title' : data.name;
            blade.subtitle = blade.isNew ? 'export.blades.generic-filter.new-subtitle' : 'export.blades.generic-filter.subtitle';
        };

        var formScope;
        $scope.setForm = function (form) { formScope = form; }

        function isDirty() {
            return !angular.equals(blade.currentEntity, blade.origEntity);
        };

        blade.headIcon = 'fa-filter';

        blade.toolbarCommands = [
                {
                    name: "export.blades.generic-filter.commands.apply", icon: 'fa fa-filter',
                    executeMethod: function () {
                        $scope.apply(false);
                    },
                    // Apply should not be avaailable when editing saved filter
                    canExecuteMethod: function () {
                        return !(blade.origEntity && blade.origEntity.id && $localStorage.exportSearchFilterIds[blade.exportTypeName] === blade.origEntity.id);
                    }
                },
                {
                    name: "core.commands.apply-filter", icon: 'fa fa-filter',
                    executeMethod: function () {
                        $scope.apply(true);
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
                }
        ];

        function deleteEntry() {
            blade.parentBlade.filter.current = null;
            $localStorage.exportSearchFilters[blade.exportTypeName].splice($localStorage.exportSearchFilters[blade.exportTypeName].indexOf(blade.origEntity), 1);
            $localStorage.exportSearchFilterIds[blade.exportTypeName] = undefined;
            blade.parentBlade.refresh();
            $scope.bladeClose();
        }

        // actions on load
        if (blade.isNew) {
            initializeBlade({ id: new Date().getTime()});
        } else {
            if (!blade.data.id) {
                angular.extend(blade.data, { id: new Date().getTime() });
            }

            initializeBlade(blade.data);
        }
    }]);
