angular.module('platformWebApp')
    .controller('platformWebApp.thumbnail.optionDetailController', ['$rootScope', '$scope', 'platformWebApp.dialogService', 'platformWebApp.bladeNavigationService', 'platformWebApp.thumbnail.api', 'platformWebApp.thumbnail.resizeMethod', function ($rootScope, $scope, dialogService, bladeNavigationService, thumbnailApi, resizeMethod) {
        var blade = $scope.blade;

        blade.resizeMethodTypes = resizeMethod.get();

        function initializeBlade(data) {
            if (blade.isNew)
                data = { resizeMethod: 'FixedSize' };

            blade.currentEntity = angular.copy(data);
            blade.origEntity = data;
            blade.isLoading = false;

            blade.title = blade.isNew ? 'platform.blades.thumbnail.blades.setting-detail.title' : data.name;
            blade.subtitle = 'platform.blades.thumbnail.blades.setting-detail.subtitle';
        };

        $scope.saveChanges = function () {
            blade.isLoading = true;

            if (blade.isNew) {
                blade.isNew = false;
                thumbnailApi.saveOption(blade.currentEntity, function (data) {
                    blade.parentBlade.refresh(true);
                    initializeBlade(data);
                }, function (error) {
                    bladeNavigationService.setError('Error: ' + error.status, blade);
                });
            } else {
                thumbnailApi.updateOption(blade.currentEntity, function (data) {
                    blade.parentBlade.refresh(true);
                    initializeBlade(data);
                }, function (error) {
                    bladeNavigationService.setError('Error: ' + error.status, blade);
                });
            }
        };

        function deleteEntry() {
            var dialog = {
                id: "confirmDelete",
                title: "core.dialogs.currency-delete.title",
                message: "core.dialogs.currency-delete.message",
                callback: function (remove) {
                    if (remove) {
                        blade.isLoading = true;

                        thumbnailApi.removeOptions({ codes: blade.currentEntity.code }, function () {
                            angular.copy(blade.currentEntity, blade.origEntity);
                            $scope.bladeClose();
                            blade.parentBlade.setSelectedId(null);
                            blade.parentBlade.refresh(true);
                        }, function (error) {
                            bladeNavigationService.setError('Error ' + error.status, blade);
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
            bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "core.dialogs.currency-save.title", "core.dialogs.currency-save.message");
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
                    return !blade.origEntity.isPrimary;
                }
            }
        ];

        initializeBlade(blade.data);

    }]);
