angular.module('virtoCommerce.contentModule')
.controller('virtoCommerce.contentModule.editBlogController', ['$rootScope', '$scope', 'platformWebApp.validators', 'virtoCommerce.contentModule.contentApi', 'platformWebApp.dynamicProperties.api', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.dynamicProperties.dictionaryItemsApi', 'platformWebApp.settings', function ($rootScope, $scope, validators, contentApi, dynamicPropertiesApi, bladeNavigationService, dialogService, dictionaryItemsApi, settings) {
    var blade = $scope.blade;
    blade.updatePermission = 'content:update';
    blade.frontMatterHeaders = 'VirtoCommerce.ContentModule.Web.Model.FrontMatterHeaders'
    $scope.validators = validators;

    blade.initialize = function () {
        if (blade.isNew) {
            fillMetadata({});
        } else {
            contentApi.getWithMetadata({
                contentType: blade.contentType,
                storeId: blade.storeId,
                relativeUrl: getBlogBlobName()
            },
            fillMetadata,
            function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
        }
    };

    function fillMetadata(data) {
        dynamicPropertiesApi.getPropertiesForType({ typeName: blade.frontMatterHeaders }, { skip: 0, take: 999 }, function (results) {
            fillDynamicProperties(data.metadata, results);
            blade.origEntity = angular.copy(blade.currentEntity);
            blade.isLoading = false;

        }, function (error) { bladeNavigationService.setError('Error ' + error.status, blade); });
    }

    function fillDynamicProperties(metadata, props) {
        _.each(props, function (x) {
            x.displayNames = undefined;
            var metadataRecord = _.findWhere(metadata, { name: x.name });
            if (metadataRecord && x.isMultilingual && !x.isDictionary) {
                metadataRecord.values = _.pluck(metadataRecord.values, 'value');
            }

            x.values = metadataRecord ? metadataRecord.values : [];
        });

        blade.currentEntity.dynamicProperties = props;
    }

    $scope.saveChanges = function () {
        blade.isLoading = true;

        contentApi.saveWithMetadata({
            contentType: blade.contentType,
            storeId: blade.storeId,
            folderUrl: ''
        }, {
            dynamicProperties: blade.currentEntity.dynamicProperties,
            content: '',
            name: getBlogBlobName()
        },
        function (data) {
            blade.isLoading = false;
            blade.origEntity = angular.copy(blade.currentEntity);
            if (blade.isNew) {
                $scope.bladeClose();
                $rootScope.$broadcast("cms-statistics-changed", blade.storeId);
            }

            blade.parentBlade.refresh();
        },
        function (error) { bladeNavigationService.setError('Error ' + error.status, $scope.blade); blade.isLoading = false; });
    };

    blade.deleteBlog = function () {
        contentApi.delete({
            contentType: blade.contentType,
            storeId: blade.storeId,
            urls: [blade.currentEntity.url]
        }, function () {
            $scope.bladeClose();
            blade.parentBlade.refresh();
        },
        function (error) { bladeNavigationService.setError('Error ' + error.status, $scope.blade); blade.isLoading = false; });
    };

    if (!blade.isNew) {
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
            }
            //{
            //    name: "platform.commands.delete", icon: 'fa fa-trash',
            //    executeMethod: function () {
            //        var dialog = {
            //            id: "confirmDeleteContentItem",
            //            title: "content.dialogs.blog-delete.title",
            //            message: "content.dialogs.blog-delete.message",
            //            callback: function (remove) {
            //                if (remove) {
            //                    blade.deleteBlog();                                
            //                }
            //            }
            //        };

            //        dialogService.showConfirmationDialog(dialog);
            //    },
            //    canExecuteMethod: function () { return true; },
            //    permission: blade.updatePermission
            //}
        ];
    }

    blade.toolbarCommands = blade.toolbarCommands || [];
    blade.toolbarCommands.push(
		{
		    name: "content.commands.manage-metadata", icon: 'fa fa-edit',
		    executeMethod: function () {
		        var newBlade = {
		            id: 'dynamicPropertyList',
		            objectType: 'VirtoCommerce.ContentModule.Web.FrontMatterHeaders',
		            parentRefresh: function (props) { fillDynamicProperties(blade.currentEntity.dynamicProperties, props); },
		            controller: 'platformWebApp.dynamicPropertyListController',
		            template: '$(Platform)/Scripts/app/dynamicProperties/blades/dynamicProperty-list.tpl.html'
		        };
		        bladeNavigationService.showBlade(newBlade, blade);
		    },
		    canExecuteMethod: function () { return true; }
		}
    );

    function getBlogBlobName() {
        return blade.currentEntity.name + '/' + blade.currentEntity.name + '.md'
    }
    function isDirty() {
        return !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
    }

    function canSave() {
        return isDirty() && formScope && formScope.$valid;
    }

    blade.onClose = function (closeCallback) {
        bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "content.dialogs.blog-save.title", "content.dialogs.blog-save.message");
    };

    var formScope;
    $scope.setForm = function (form) { $scope.formScope = formScope = form; };

    $scope.editDictionary = function (property) {
        var newBlade = {
            id: "propertyDictionary",
            isApiSave: true,
            currentEntity: property,
            controller: 'platformWebApp.propertyDictionaryController',
            template: '$(Platform)/Scripts/app/dynamicProperties/blades/property-dictionary.tpl.html',
            onChangesConfirmedFn: function () {
                dictionaryItemsApi.query({ id: property.objectType, propertyId: property.id }, getDictionaryValuesCallback);
            }
        };
        bladeNavigationService.showBlade(newBlade, blade);
    };

    var getDictionaryValuesCallback;
    $scope.getDictionaryValues = function (property, callback) {
        getDictionaryValuesCallback = callback;
        dictionaryItemsApi.query({ id: property.objectType, propertyId: property.id }, callback);
    }

    $scope.languages = settings.getValues({ id: 'VirtoCommerce.Core.General.Languages' });

    blade.headIcon = 'fa-inbox';

    blade.initialize();
}]);
