angular.module('virtoCommerce.inventoryModule')
    .controller('virtoCommerce.inventoryModule.fulfillmentCenterDetailController', ['$scope', 'platformWebApp.dialogService', 'platformWebApp.bladeNavigationService', 'virtoCommerce.inventoryModule.fulfillments', 'platformWebApp.common.countries', function ($scope, dialogService, bladeNavigationService, fulfillments, countries) {
        var blade = $scope.blade;
        blade.updatePermission = 'inventory:fulfillment:edit';
        blade.refresh = function (parentRefresh) {
            if (blade.currentEntityId) {
                blade.isLoading = true;
                fulfillments.get({ id: blade.currentEntityId }, function (data) {
                    initializeBlade(data);
                    if (parentRefresh) {
                        blade.parentBlade.refresh();
                    }
                });
            }
            else {
                initializeBlade(blade.currentEntity);
            }
        }

        function initializeBlade(data) {
            blade.currentEntity = angular.copy(data);
            blade.origEntity = data;
            blade.isLoading = false;
        };

        $scope.setForm = function (form) {
            $scope.formScope = form;
        };

        function isDirty() {
            return !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
        }

        function canSave() {
            return isDirty() && $scope.formScope && $scope.formScope.$valid;
        }

        function saveChanges() {
            blade.isLoading = true;
            fulfillments.update({}, blade.currentEntity, function (center) {
                blade.currentEntityId = center.id;
                blade.refresh(true);
            });
        }

        blade.headIcon = 'fa-wrench';

        blade.toolbarCommands = [
            {
                name: "platform.commands.save", icon: 'fa fa-save',
                executeMethod: saveChanges,
                canExecuteMethod: canSave,
                permission: blade.updatePermission
            },
            {
                name: "platform.commands.reset", icon: 'fa fa-undo',
                executeMethod: function () {
                    angular.copy(blade.origEntity, blade.currentEntity);
                },
                canExecuteMethod: isDirty,
                permission: blade.updatePermission
            },
            {
                name: "platform.commands.delete", icon: 'fa fa-trash-o',
                executeMethod: deleteEntry,
                canExecuteMethod: function () { return blade.currentEntity.id != null && blade.currentEntity.id != undefined; },
                permission: 'inventory:fulfillment:delete'
            }
        ];

        function deleteEntry() {
            var dialog = {
                id: "confirmDelete",
                title: "inventory.dialogs.fulfillment-delete.title",
                message: "inventory.dialogs.fulfillment-delete.message",
                callback: function (remove) {
                    if (remove) {
                        blade.isLoading = true;

                        fulfillments.remove({ ids: blade.currentEntityId }, function () {
                            $scope.bladeClose();
                            blade.parentBlade.refresh(true);
                        }, function (error) {
                            bladeNavigationService.setError('Error ' + error.status, blade);
                        });
                    }
                }
            }
            dialogService.showConfirmationDialog(dialog);
        }

        blade.onClose = function (closeCallback) {
            bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, saveChanges, closeCallback, "core.dialogs.fulfillments-save.title", "core.dialogs.fulfillments-save.message");
        };

        // actions on load
        blade.refresh();

    }]);
