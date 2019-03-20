angular.module('virtoCommerce.marketingModule')
.controller('virtoCommerce.marketingModule.itemFolderDetailController', ['$scope', 'virtoCommerce.marketingModule.dynamicContent.folders', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', function ($scope, marketing_dynamicContents_res_folders, bladeNavigationService, dialogService) {
    var blade = $scope.blade;
    blade.updatePermission = 'marketing:update';
    
    blade.saveChanges = function () {
        blade.isLoading = true;

        if (blade.isNew) {
            marketing_dynamicContents_res_folders.save(blade.currentEntity, function (data) {
                blade.parentBlade.initializeBlade();
                bladeNavigationService.closeBlade(blade);
                blade.isLoading = false;
            });
        }
        else {
            marketing_dynamicContents_res_folders.update(blade.currentEntity, function () {
                blade.parentBlade.initializeBlade();
                blade.origEntity = angular.copy(blade.currentEntity);
                blade.isLoading = false;
            });
        }
    };

    blade.headIcon = 'fa-inbox';

    if (!blade.isNew) {
        blade.toolbarCommands = [
            {
                name: "platform.commands.save", icon: 'fa fa-save',
                executeMethod: blade.saveChanges,
                canExecuteMethod: function () {
                    return !angular.equals(blade.origEntity, blade.currentEntity) && !$scope.formScope.$invalid;
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
                        title: "marketing.dialogs.content-folder-delete.title",
                        message: "marketing.dialogs.content-folder-delete.message",
                        callback: function (remove) {
                            if (remove) {
                                blade.isLoading = true;
                                marketing_dynamicContents_res_folders.delete({ ids: [blade.currentEntity.id] }, function () {
                                    bladeNavigationService.closeBlade(blade);

                                    var pathSteps = blade.currentEntity.outline.split(';');
                                    var id = pathSteps[pathSteps.length - 2];
                                    blade.parentBlade.chosenFolder = id;
                                    blade.parentBlade.initializeBlade();
                                });
                            }
                        }
                    };
                    dialogService.showConfirmationDialog(dialog);
                },
                canExecuteMethod: function () { return true; },
                permission: blade.updatePermission
            }
        ];
    }

    $scope.setForm = function (form) { $scope.formScope = form; };
    
    blade.origEntity = blade.entity;
    blade.currentEntity = angular.copy(blade.origEntity);
    blade.isLoading = false;
}]);