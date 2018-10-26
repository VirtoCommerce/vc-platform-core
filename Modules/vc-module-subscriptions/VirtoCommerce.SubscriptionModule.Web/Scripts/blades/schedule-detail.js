angular.module('virtoCommerce.subscriptionModule')
.controller('virtoCommerce.subscriptionModule.scheduleDetailController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'virtoCommerce.subscriptionModule.scheduleAPI', function ($scope, bladeNavigationService, dialogService, scheduleAPI) {
    var blade = $scope.blade;
    blade.updatePermission = 'subscription:update';
    blade.isNew = !blade.data.id;

    blade.initialize = function (data) {
        if (blade.isNew) {
            angular.extend(data, {
                intervalCount: 1,
                interval: $scope.intervals[2]
            });
        }

        blade.origEntity = data;
        blade.currentEntity = angular.copy(data);
        blade.isLoading = false;
    };

    $scope.intervals = ['days', 'weeks', 'months', 'years'];

    function isDirty() {
        return !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
    }

    function canSave() {
        return isDirty() && $scope.formScope.$valid;
    }

    $scope.setForm = function (form) { $scope.formScope = form; };

    blade.onClose = function (closeCallback) {
        bladeNavigationService.showConfirmationIfNeeded(isDirty() && !blade.isNew, canSave(), blade, $scope.saveChanges, closeCallback, "subscription.dialogs.schedule-save.title", "subscription.dialogs.schedule-save.message");
    };

    $scope.cancelChanges = function () {
        blade.currentEntity = blade.origEntity;
        $scope.bladeClose();
    };
    $scope.saveChanges = function () {
        if (blade.isApiSave) {
            blade.isLoading = true;

            if (blade.isNew) {
                blade.currentEntity.id = blade.itemId;
            }

            scheduleAPI.save(blade.currentEntity, function () {
                angular.copy(blade.currentEntity, blade.origEntity);
                $scope.bladeClose();
            });
        } else {
            angular.copy(blade.currentEntity, blade.origEntity);
            $scope.bladeClose();
        }
    };

    blade.toolbarCommands = [
        {
            name: "platform.commands.save", icon: 'fa fa-save',
            executeMethod: $scope.saveChanges,
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
            executeMethod: function () {
                dialogService.showConfirmationDialog({
                    id: "confirmDeleteItem",
                    title: "subscription.dialogs.schedule-delete.title",
                    message: "subscription.dialogs.schedule-delete.message",
                    callback: function (remove) {
                        if (remove) {
                            scheduleAPI.delete({ ids: [blade.origEntity.id] }, function () {
                                blade.currentEntity = {};
                                angular.copy(blade.currentEntity, blade.origEntity);
                                $scope.bladeClose();
                            });
                        }
                    }
                });
            },
            canExecuteMethod: function () { return true; },
            permission: 'subscription:delete'
        }
    ];

    if (blade.isNew || !blade.isApiSave) {
        $scope.blade.toolbarCommands.splice(0, 1); // remove save button
        $scope.blade.toolbarCommands.splice(1, 1); // remove delete
    }

    if (blade.isNew) {
        angular.extend(blade, {
            title: 'subscription.blades.schedule-detail.title-new',
            subtitle: 'subscription.blades.schedule-detail.subtitle-new'
        });
    } else {
        angular.extend(blade, {
            title: 'subscription.blades.schedule-detail.title'
        });
    }

    blade.initialize(blade.data);
}]);