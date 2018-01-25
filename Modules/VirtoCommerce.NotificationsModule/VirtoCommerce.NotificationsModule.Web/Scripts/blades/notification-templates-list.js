angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.notificationTemplatesListController', ['$scope', 'virtoCommerce.notificationsModule.notificationsService', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.notifications', 'platformWebApp.settings', 'platformWebApp.ui-grid.extension', 'platformWebApp.uiGridHelper', 'platformWebApp.bladeUtils',
    function ($scope, notificationsService, bladeNavigationService, dialogService, notifications, settings, gridOptionExtension, uiGridHelper, bladeUtils) {
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
        
        blade.openTemplate = function (template) {
		    blade.selectedLanguage = template.language;
            var newBlade = {
                id: 'editTemplate',
                title: 'notifications.blades.notifications-edit-template.title',
                titleValues: { displayName: template.displayName },
                subtitle: 'notifications.blades.notifications-edit-template.subtitle',
                templateId: template.id,
                notificationType: blade.notificationType,
                objectId: blade.objectId,
                objectTypeId: blade.objectTypeId,
                isNew: false,
                isFirst: false,
                languages: blade.languages,
                controller: 'virtoCommerce.notificationsModule.editTemplateController',
                template: 'Modules/$(virtoCommerce.notificationsModule)/Scripts/blades/notifications-edit-template.tpl.html'
            };

            bladeNavigationService.showBlade(newBlade, blade);    
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
		    var newBlade = {
			    id: 'editTemplate',
			    title: 'notifications.blades.notifications-edit-template.title-new',
                titleValues: { displayName: template.displayName },
			    notificationType: template.notificationType,
                displayName: template.displayName,
			    objectId: blade.objectId,
			    objectTypeId: blade.objectTypeId,
			    language: 'undefined',
			    isNew: true,
			    isFirst: false,
			    languages: blade.languages,
			    controller: 'virtoCommerce.notificationsModule.editTemplateController',
			    template: 'Modules/$(virtoCommerce.notificationsModule)/Scripts/blades/notifications-edit-template.tpl.html'
		    };

		    bladeNavigationService.showBlade(newBlade, blade);
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
