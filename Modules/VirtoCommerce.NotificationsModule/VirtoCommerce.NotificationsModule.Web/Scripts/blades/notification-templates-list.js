angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.notificationTemplatesListController', ['$scope', '$translate', 'virtoCommerce.notificationsModule.notificationTypesResolverService', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.settings', 'platformWebApp.ui-grid.extension', 'platformWebApp.uiGridHelper', 'platformWebApp.bladeUtils',
    function ($scope, $translate, notificationTypesResolverService, bladeNavigationService, dialogService, settings, gridOptionExtension, uiGridHelper, bladeUtils) {
        $scope.uiGridConstants = uiGridHelper.uiGridConstants;
	    var blade = $scope.blade;
	    blade.selectedLanguage = null;

	    if (!blade.languages) {
            var languages = ["default", "en-US", "de-DE"];
            blade.languages = languages;
            //TODO
		    //blade.languages = settings.getValues({ id: 'VirtoCommerce.Core.General.Languages' });
	    }

	    blade.initialize = function () {
		    blade.isLoading = true;
            blade.currentEntities = blade.currentEntity.templates;
            blade.isLoading = false;
        }
        
        function resolveType(kind) {
            var foundTemplate = notificationTypesResolverService.resolve(kind);
            if (foundTemplate && foundTemplate.knownChildrenTypes && foundTemplate.knownChildrenTypes.length) {
                    return foundTemplate;
                } else {
                    dialogService.showNotificationDialog({
                        id: "error",
                        title: "notifications.dialogs.unknown-sendGateway-type.title",
                        message: "notifications.dialogs.unknown-sendGateway-type.message",
                        messageValues: { kind: kind },
                });
            }
        }
        
        blade.openTemplate = function (template) {
		    var foundTemplate = resolveType(blade.currentEntity.kind);
            if (foundTemplate) {
                var newBlade = {
                    id: foundTemplate.detailBlade.id,
                    title: 'notifications.blades.notifications-edit-template.title',
                    titleValues: { displayName: $translate.instant('notificationTypes.' + blade.currentEntity.type + '.displayName') },
                    languageCode: template.languageCode,
                    notification: blade.currentEntity,
                    isNew: false,
                    isFirst: false,
                    languages: blade.languages,
                    objectId: blade.objectId,
                    objectTypeId: blade.objectTypeId,
                    controller: foundTemplate.detailBlade.controller,
                    template: foundTemplate.detailBlade.template
                };

                bladeNavigationService.showBlade(newBlade, blade);
            } 
	    }

        blade.setSelectedNode = function (listItem) {
            $scope.selectedNodeId = listItem.id;
        };

        $scope.selectNode = function (type) {
            blade.setSelectedNode(type);
            blade.selectedType = type;
            blade.openTemplate(type);
        };
                
	    function createTemplate(template) {
            var foundTemplate = resolveType(blade.currentEntity.kind);
            if (foundTemplate) {
                var newBlade = {
                    id: foundTemplate.detailBlade.id,
                    title: 'notifications.blades.notifications-edit-template.title-new',
                    titleValues: { displayName: $translate.instant('notificationTypes.' + blade.currentEntity.type + '.displayName') },
                    notification: blade.currentEntity,
                    objectId: blade.objectId,
                    objectTypeId: blade.objectTypeId,
                    languageCode: null,
                    isNew: true,
                    isFirst: false,
                    languages: blade.languages,
                    controller: foundTemplate.detailBlade.controller,
                    template: foundTemplate.detailBlade.template
                };

                bladeNavigationService.showBlade(newBlade, blade);
            }
	    }

	    blade.toolbarCommands = [
			    {
				    name: "platform.commands.add", icon: 'fa fa-plus',
				    executeMethod: createTemplate,
				    canExecuteMethod: function () { return true; },
				    permission: 'platform:notification:create'
			    }
	    ];

        // ui-grid
        $scope.setGridOptions = function (gridOptions) {
          uiGridHelper.initialize($scope, gridOptions, function (gridApi) {
              uiGridHelper.bindRefreshOnSortChanged($scope);
          });
          bladeUtils.initializePagination($scope);
        };

        blade.initialize();
}]);
