angular.module('virtoCommerce.catalogModule')
    .controller('virtoCommerce.catalogModule.propertyDetailController', ['$scope', 'virtoCommerce.catalogModule.properties', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'virtoCommerce.catalogModule.valueTypes', function ($scope, properties, bladeNavigationService, dialogService, valueTypes) {
        var blade = $scope.blade;
        blade.updatePermission = 'catalog:update';
        blade.origEntity = {};
        $scope.currentChild = undefined;
        blade.title = "catalog.blades.property-detail.title";
        blade.subtitle = "catalog.blades.property-detail.subtitle";
        blade.availableValueTypes = valueTypes.get();

        blade.availablePropertyTypes = blade.catalogId ? ['Product', 'Variation', 'Category', 'Catalog'] : ['Product', 'Variation', 'Category'];

        blade.refresh = function (parentRefresh) {
            if (blade.currentEntityId) {
                properties.get({ propertyId: blade.currentEntityId }, function (data) {
                    initializeBlade(data);
                    if (parentRefresh) {
                        blade.parentBlade.refresh();
                    }
                },
                    function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
            } else if (blade.categoryId) {
                properties.newCategoryProperty({ categoryId: blade.categoryId }, function (data) {
                    initializeBlade(data);
                },
                    function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
            }
            else if (blade.catalogId) {
                properties.newCatalogProperty({ catalogId: blade.catalogId }, function (data) {
                    initializeBlade(data);
                },
                    function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
            }
        };

        $scope.openChild = function (childType) {
            var newBlade = { id: "propertyChild" };
            newBlade.property = blade.currentEntity;
            switch (childType) {
                case 'attr':
                    newBlade.title = 'catalog.blades.property-attributes.title';
                    newBlade.titleValues = { name: blade.origEntity.name ? blade.origEntity.name : blade.currentEntity.name };
                    newBlade.subtitle = 'catalog.blades.property-attributes.subtitle';
                    newBlade.controller = 'virtoCommerce.catalogModule.propertyAttributesController';
                    newBlade.template = 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/property-attributes.tpl.html';
                    break;
                case 'rules':
                    newBlade.title = 'catalog.blades.property-validationRule.title';
                    newBlade.titleValues = { name: blade.origEntity.name ? blade.origEntity.name : blade.currentEntity.name };
                    newBlade.subtitle = 'catalog.blades.property-validationRule.subtitle';
                    newBlade.controller = 'virtoCommerce.catalogModule.propertyValidationRulesController';
                    newBlade.template = 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/property-validationRules.tpl.html';
                    break;
                case 'dict':
                    newBlade.title = 'catalog.blades.property-dictionary.title';
                    newBlade.titleValues = { name: blade.origEntity.name ? blade.origEntity.name : blade.currentEntity.name };
                    newBlade.subtitle = 'catalog.blades.property-dictionary.subtitle';
                    newBlade.controller = 'virtoCommerce.catalogModule.propertyDictionaryController';
                    newBlade.template = 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/property-dictionary.tpl.html';
                    break;
            }
            bladeNavigationService.showBlade(newBlade, blade);
            $scope.currentChild = childType;
        }

        function initializeBlade(data) {
            if (data.valueType === 'Number' && data.dictionaryValues) {
                _.forEach(data.dictionaryValues, function (entry) {
                    entry.value = parseFloat(entry.value);
                });
            }

            blade.currentEntity = angular.copy(data);
            blade.origEntity = data;
            blade.isLoading = false;
        };

        function isDirty() {
            return !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
        }

        function canSave() {
            return (blade.origEntity.isNew || isDirty()) && formScope && formScope.$valid;
        }

        function saveChanges() {
            blade.isLoading = true;

            if (blade.currentEntity.valueType !== "ShortText" && blade.currentEntity.valueType !== "LongText") {
                blade.currentEntity.validationRule = null;
            }

            properties.update(blade.currentEntity, function (data, headers) {
                blade.currentEntityId = data.id;
                blade.refresh(true);
            });
        };

        function removeProperty(prop) {
            var dialog = {
                id: "confirmDelete",
                messageValues: { name: prop.name },
                callback: function (doDeleteValues) {
                    blade.isLoading = true;

                    properties.remove({ id: prop.id, doDeleteValues: doDeleteValues }, function () {
                        $scope.bladeClose();
                        blade.parentBlade.refresh();
                    });
                }
            };
            dialogService.showDialog(dialog, 'Modules/$(VirtoCommerce.Catalog)/Scripts/dialogs/deleteProperty-dialog.tpl.html', 'platformWebApp.confirmDialogController');
        }

        blade.onClose = function (closeCallback) {
            bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, saveChanges, closeCallback, "catalog.dialogs.property-save.title", "catalog.dialogs.property-save.message");
        };

        var formScope;
        $scope.setForm = function (form) { formScope = form; }

        blade.headIcon = 'fa-gear';

        blade.toolbarCommands = [
            {
                name: "platform.commands.save", icon: 'fa fa-save',
                executeMethod: saveChanges,
                canExecuteMethod: canSave
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
                executeMethod: function () {
                    removeProperty(blade.origEntity);
                },
                canExecuteMethod: function () {
                    return blade.origEntity.isManageable && !blade.origEntity.isNew;
                }
            }
        ];

        // actions on load    
        blade.refresh();
    }]);
