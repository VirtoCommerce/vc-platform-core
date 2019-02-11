angular.module('virtoCommerce.marketingModule')
.controller('virtoCommerce.marketingModule.placeholderDetailController', ['$scope', 'virtoCommerce.marketingModule.dynamicContent.contentPlaces', 'platformWebApp.bladeNavigationService', 'FileUploader', 'platformWebApp.dialogService', function ($scope, contentPlacesApi, bladeNavigationService, FileUploader, dialogService) {
    var blade = $scope.blade;
    blade.updatePermission = 'marketing:update';

    blade.origEntity = blade.entity;
    blade.currentEntity = angular.copy(blade.origEntity);

    blade.initialize = function () {
        if (!$scope.uploader && blade.hasUpdatePermission()) {
            // create the uploader
            var uploader = $scope.uploader = new FileUploader({
                scope: $scope,
                headers: { Accept: 'application/json' },
                url: 'api/platform/assets?folderUrl=placeholders-images',
                autoUpload: true,
                removeAfterUpload: true
            });

            uploader.onSuccessItem = function (fileItem, images, status, headers) {
                blade.currentEntity.imageUrl = images[0].url;
            };

            uploader.onAfterAddingAll = function (addedItems) {
                bladeNavigationService.setError(null, blade);
            };

            uploader.onErrorItem = function (item, response, status, headers) {
                bladeNavigationService.setError(item._file.name + ' failed: ' + (response.message ? response.message : status), blade);
            };
        }

        if (!blade.isNew) {
            $scope.blade.toolbarCommands = [
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
				            id: "confirmDeleteContentPlaceholder",
				            title: "marketing.dialogs.content-placeholder-delete.title",
				            message: "marketing.dialogs.content-placeholder-delete.message",
				            callback: function (remove) {
				                if (remove) {
				                    blade.isLoading = true;
				                    contentPlacesApi.delete({ ids: [blade.currentEntity.id] }, function () {
				                        blade.parentBlade.initialize();
				                        bladeNavigationService.closeBlade(blade);
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

        blade.isLoading = false;
    }

    blade.saveChanges = function () {
        blade.isLoading = true;

        if (blade.isNew) {
            contentPlacesApi.save({}, blade.currentEntity, function (data) {
                blade.parentBlade.initialize();
                bladeNavigationService.closeBlade(blade);
            });
        }
        else {
            contentPlacesApi.update({}, blade.currentEntity, function (data) {
                blade.parentBlade.initialize();
                blade.origEntity = angular.copy(blade.currentEntity);
                blade.isLoading = false;
            });
        }
    };

    $scope.setForm = function (form) { $scope.formScope = form; };

    $scope.blade.headIcon = 'fa-location-arrow';

    blade.initialize();
}]);