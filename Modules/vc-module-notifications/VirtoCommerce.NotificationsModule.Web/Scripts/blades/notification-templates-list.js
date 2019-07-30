angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.notificationTemplatesListController', ['$scope', '$translate', '$filter', 'virtoCommerce.notificationsModule.notificationTemplatesResolverService', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.settings', 'platformWebApp.ui-grid.extension', 'platformWebApp.uiGridHelper', 'platformWebApp.bladeUtils',
    function ($scope, $translate, $filter, notificationTemplatesResolverService, bladeNavigationService, dialogService, settings, gridOptionExtension, uiGridHelper, bladeUtils) {
        $scope.uiGridConstants = uiGridHelper.uiGridConstants;
	    var blade = $scope.blade;
	    blade.selectedLanguage = null;

	    if (!blade.languages) {
            settings.getValues({ id: 'VirtoCommerce.Notifications.General.Languages' }, function (data) {
                blade.languages = data;
            });
	    }

	    blade.initialize = function () {
		    blade.isLoading = true;
            blade.currentEntities = blade.currentEntity.templates;
            blade.isLoading = false;
        }
        
        function resolveType(kind) {
            var foundTemplate = notificationTemplatesResolverService.resolve(kind);
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
                if (!template.isReadonly) {
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
                        template: foundTemplate.detailBlade.template,
                        kind: blade.currentEntity.kind
                    };
                } else {
                    var newBlade = {
                        id: 'renderTemplate',
                        title: 'notifications.blades.notifications-template-render.title',
                        subtitle: 'notifications.blades.notifications-template-render.subtitle',
                        subtitleValues: { type: blade.currentEntity.notificationType },
                        notification: blade.currentEntity,
                        tenantId: blade.tenantId,
                        tenantType: blade.tenantType,
                        currentEntity: template,
                        language: template.languageCode,
                        controller: 'virtoCommerce.notificationsModule.templateRenderController',
                        template: 'Modules/$(VirtoCommerce.Notifications)/Scripts/blades/notifications-template-render.tpl.html'
                    };
                }
                
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
				    permission: 'notifications:template:create'
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
