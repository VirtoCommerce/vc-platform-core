angular.module('virtoCommerce.marketingModule')
.controller('virtoCommerce.marketingModule.itemDetailController', ['$scope', 'virtoCommerce.marketingModule.dynamicContent.contentItems', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.dynamicProperties.dictionaryItemsApi', 'platformWebApp.settings',
    function ($scope, dynamicContentItemsApi, bladeNavigationService, dialogService, dictionaryItemsApi, settings) {
        var blade = $scope.blade;
        blade.updatePermission = 'marketing:update';
       
        blade.initialize = function () {
            blade.toolbarCommands = [];

            if (!blade.isNew) {
                blade.toolbarCommands = [
                    {
                        name: "platform.commands.save", icon: 'fa fa-save',
                        executeMethod: function () {
                            blade.saveChanges();
                        },
                        canExecuteMethod: function () {
                            return !angular.equals(blade.origEntity, blade.currentEntity) && $scope.formScope.$valid;
                        },
                        permission: blade.updatePermission
                    },
                    {
                        name: "platform.commands.reset", icon: 'fa fa-undo',
                        executeMethod: function () {
                            angular.copy(blade.origEntity, blade.currentEntity);
                        },
                        canExecuteMethod: function () {
                            return !angular.equals(blade.origEntity, blade.currentEntity);
                        },
                        permission: blade.updatePermission
                    },
                    {
                        name: "platform.commands.delete", icon: 'fa fa-trash-o',
                        executeMethod: function () {
                            var dialog = {
                                id: "confirmDeleteContentItem",
                                title: "marketing.dialogs.content-item-delete.title",
                                message: "marketing.dialogs.content-item-delete.message",
                                callback: function (remove) {
                                    if (remove) {
                                        blade.isLoading = true;
                                        dynamicContentItemsApi.delete({ ids: [blade.currentEntity.id] }, function () {
                                            blade.parentBlade.initializeBlade();
                                            bladeNavigationService.closeBlade(blade);
                                        });
                                    }
                                }
                            };

                            dialogService.showConfirmationDialog(dialog);
                        },
                        canExecuteMethod: function () {
                            return true;
                        },
                        permission: blade.updatePermission
                    }
                ];
            }

            blade.toolbarCommands.push(
                {
                    name: "marketing.commands.manage-type-properties", icon: 'fa fa-edit',
                    executeMethod: function () {
                        var newBlade = {
                            id: 'dynamicPropertyList',
                            objectType: blade.currentEntity.objectType,
                            controller: 'platformWebApp.dynamicPropertyListController',
                            template: '$(Platform)/Scripts/app/dynamicProperties/blades/dynamicProperty-list.tpl.html'
                        };
                        bladeNavigationService.showBlade(newBlade, blade);
                    },
                    canExecuteMethod: function () {
                        return angular.isDefined(blade.currentEntity.objectType);
                    }
                });

            blade.origEntity = blade.entity;
            blade.currentEntity = angular.copy(blade.origEntity);
            blade.isLoading = false;
        };
        
        blade.saveChanges = function () {
            blade.isLoading = true;

            if (blade.isNew) {
                dynamicContentItemsApi.save(blade.currentEntity, function (data) {
                    blade.parentBlade.initializeBlade();
                    bladeNavigationService.closeBlade(blade);
                });
            }
            else {
                dynamicContentItemsApi.update(blade.currentEntity, function (data) {
                    blade.parentBlade.initializeBlade();
                    blade.origEntity = angular.copy(blade.currentEntity);
                    blade.isLoading = false;
                });
            }
        };

        $scope.editDictionary = function (property) {
            var newBlade = {
                id: "propertyDictionary",
                isApiSave: true,
                currentEntity: property,
                controller: 'platformWebApp.propertyDictionaryController',
                template: '$(Platform)/Scripts/app/dynamicProperties/blades/property-dictionary.tpl.html',
                onChangesConfirmedFn: function () {
                    blade.currentEntity.dynamicProperties = angular.copy(blade.currentEntity.dynamicProperties);
                }
            };
            bladeNavigationService.showBlade(newBlade, blade);
        };

        $scope.getDictionaryValues = function (property, callback) {
            dictionaryItemsApi.query({ id: property.objectType, propertyId: property.id }, callback);
        };

        $scope.setForm = function (form) { $scope.formScope = form; };

        blade.headIcon = 'fa-inbox';

        settings.getValues({ id: 'VirtoCommerce.Core.General.Languages' }, function (data) {
            $scope.languages = data;
        });

        blade.initialize();
    }]);