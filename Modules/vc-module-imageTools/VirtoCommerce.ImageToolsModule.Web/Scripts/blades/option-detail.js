angular.module('virtoCommerce.imageToolsModule')
    .controller('virtoCommerce.imageToolsModule.optionDetailController', ['$rootScope', '$scope', 'platformWebApp.dialogService', 'platformWebApp.bladeNavigationService', 'virtoCommerce.imageToolsModule.resizeMethod', 'virtoCommerce.imageToolsModule.anchorPosition', 'virtoCommerce.imageToolsModule.jpegQuality', 'virtoCommerce.imageToolsModule.optionApi', function ($rootScope, $scope, dialogService, bladeNavigationService, resizeMethod, anchorPosition, jpegQuality, optionApi) {
        var blade = $scope.blade;

        $scope.positiveNum = /^[0-9]+$/;

        blade.resizeMethodTypes = resizeMethod.get();
        blade.anchorPositionTypes = anchorPosition.get();
        blade.jpegQualityTypes = jpegQuality.get();

        blade.refresh = function (parentRefresh) {
            blade.isLoading = true;
            if (blade.isNew) {
                initializeBlade({ resizeMethod: 'FixedSize', anchorPosition: 'Center', jpegQuality: 'High' });
            } else {
                optionApi.get({ id: blade.currentEntityId }, function (data) {

                    initializeBlade(data);

                    if (parentRefresh) {
                        blade.parentBlade.refresh(parentRefresh);
                    }
                });
            }
        }

        function initializeBlade(data) {
            blade.item = angular.copy(data);
            blade.currentEntity = blade.item;
            blade.origEntity = data;
            blade.isLoading = false;

            blade.title = blade.isNew ? 'imageTools.blades.setting-detail.title' : data.name;
            blade.subtitle = 'imageTools.blades.setting-detail.subtitle';
        };

        $scope.saveChanges = function () {
            blade.isLoading = true;
            var promise = saveOrUpdate();
            promise.catch(function (error) {
                bladeNavigationService.setError('Error ' + error.status, blade);
            }).finally(function () {
                blade.isLoading = false;
            });
        };

        function saveOrUpdate() {
            if (blade.isNew) {
                return optionApi.save(blade.currentEntity, function (data) {
                    blade.isNew = false;
                    blade.currentEntityId = data.id;
                    blade.refresh(true);
                }).$promise;
            } else {
                return optionApi.update(blade.currentEntity, function () {
                    blade.refresh(true);
                }).$promise;
            }
        }

        function deleteEntry() {
            var dialog = {
                id: "confirmDelete",
                title: "imageTools.dialogs.setting-delete.title",
                message: "imageTools.dialogs.setting-delete.message",
                callback: function (remove) {
                    if (remove) {
                        blade.isLoading = true;
                        optionApi.delete({ ids: blade.currentEntityId }, function () {
                            bladeNavigationService.closeBlade(blade, function () {
                                blade.parentBlade.refresh(true);
                            });
                        });
                    }
                }
            }
            dialogService.showConfirmationDialog(dialog);
        }

        var detailForm;
        $scope.setForm = function (form) {
            detailForm = form;
        }

        function isDirty() {
            return !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
        }

        function canSave() {
            return isDirty() && detailForm && detailForm.$valid;
        }


        blade.onClose = function (closeCallback) {
            bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "imageTools.dialogs.setting-save.title", "imageTools.dialogs.setting-save.message");
        };

        blade.toolbarCommands = [
            {
                name: "platform.commands.save", icon: 'fa fa-save',
                executeMethod: $scope.saveChanges,
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
                executeMethod: deleteEntry,
                canExecuteMethod: function () {
                    return !blade.isNew;
                }
            }
        ];

        blade.refresh();

    }]);
