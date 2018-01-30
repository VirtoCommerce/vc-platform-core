angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.notificationTemplatesListController', ['$scope', '$translate', 'virtoCommerce.notificationsModule.notificationsService', 'virtoCommerce.notificationsModule.notificationTypesResolverService', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.notifications', 'platformWebApp.settings', 'platformWebApp.ui-grid.extension', 'platformWebApp.uiGridHelper', 'platformWebApp.bladeUtils',
    function ($scope, $translate, notificationsService, notificationTypesResolverService, bladeNavigationService, dialogService, notifications, settings, gridOptionExtension, uiGridHelper, bladeUtils) {
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
            notificationsService.getTemplates({notificationType: blade.notificationType}).then(function (data) {
                blade.isLoading = false;
                blade.currentEntities = data;
            })
            //TODO use resources
		    // notifications.getTemplates({ type: blade.notificationType, objectId: blade.objectId, objectTypeId: blade.objectTypeId }, function (data) {
		    // 	blade.currentEntities = data;
		    // 	if (blade.currentEntities.length < 1) {
		    // 		bladeNavigationService.closeBlade(blade);
		    // 	}
		    // 	blade.isLoading = false;
		    // });
        }
        
        function resolveType(sendGatewayType) {
            var foundTemplate = notificationTypesResolverService.resolve(sendGatewayType);
            if (foundTemplate && foundTemplate.knownChildrenTypes && foundTemplate.knownChildrenTypes.length) {
                    return foundTemplate;
                } else {
                    dialogService.showNotificationDialog({
                        id: "error",
                        title: "notifications.dialogs.unknown-sendGateway-type.title",
                        message: "notifications.dialogs.unknown-sendGateway-type.message",
                        messageValues: { sendGatewayType: sendGatewayType },
                });
            }
        }
        
        blade.openTemplate = function (template) {
		    blade.selectedLanguage = template.language;
            var foundTemplate = resolveType(template.sendGatewayType);
            if (foundTemplate) {
                var newBlade = {
                    id: foundTemplate.detailBlade.id,
                    title: 'notifications.blades.notifications-edit-template.title',
                    titleValues: { displayName: $translate.instant(template.displayName) },
                    notificationType: blade.notificationType,
                    templateId: template.id,
                    isNew: false,
                    isFirst: false,
                    languages: blade.languages,
                    sendGatewayType: blade.sendGatewayType,
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
            var foundTemplate = resolveType(template.sendGatewayType);
            if (foundTemplate) {
                var newBlade = {
                    id: foundTemplate.detailBlade.id,
                    title: 'notifications.blades.notifications-edit-template.title-new',
                    titleValues: { displayName: $translate.instant(template.displayName) },
                    notificationType: template.notificationType,
                    displayName: template.displayName,
                    objectId: blade.objectId,
                    objectTypeId: blade.objectTypeId,
                    language: 'undefined',
                    isNew: true,
                    isFirst: false,
                    languages: blade.languages,
                    sendGatewayType: blade.sendGatewayType,
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
