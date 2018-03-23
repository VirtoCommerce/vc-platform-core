angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.notificationTemplatesListController', ['$scope', '$translate', '$filter', 'virtoCommerce.notificationsModule.notificationTypesResolverService', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.settings', 'platformWebApp.ui-grid.extension', 'platformWebApp.uiGridHelper', 'platformWebApp.bladeUtils',
    function ($scope, $translate, $filter, notificationTypesResolverService, bladeNavigationService, dialogService, settings, gridOptionExtension, uiGridHelper, bladeUtils) {
        $scope.uiGridConstants = uiGridHelper.uiGridConstants;
	    var blade = $scope.blade;
	    blade.selectedLanguage = null;

	    if (!blade.languages) {
            var languages = ["en-US", "de-DE"];
            blade.languages = languages;
            //TODO
		    //blade.languages = settings.getValues({ id: 'VirtoCommerce.Core.General.Languages' });
	    }

	    blade.initialize = function () {
		    blade.isLoading = true;
            blade.currentEntities = blade.currentEntity.templates;
            _.map(blade.currentEntities, function (item) {
                item.createdDateAsString = $filter('date')(item.createdDate, "yyyy-MM-dd"); 
                item.modifiedDateAsString = $filter('date')(item.modifiedDate, "yyyy-MM-dd"); 
                return item;
            });
            blade.isLoading = false;
        }
        
        function resolveType(kind) {
            var foundTemplate = notificationTypesResolverService.resolve(kind);
            if (foundTemplate && foundTemplate.knownChildrenTypes && foundTemplate.knownChildrenTypes.length) {
                    return foundTemplate;
                } else {
                    dialogService.showNotificationDialog({
                        id: "error",
                        title: "notifications.dialogs.unknown-kind.title",
                        message: "notifications.dialogs.unknown-kind.message",
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
                    notification: blade.currentEntity,
                    languageCode: template.languageCode,
                    isNew: false,
                    isFirst: false,
                    languages: blade.languages,
                    tenantId: blade.tenantId,
                    tenantType: blade.tenantType,
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
                    tenantId: blade.tenantId,
                    tenantType: blade.tenantType,
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
				    permission: 'notications:notification:create'
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
