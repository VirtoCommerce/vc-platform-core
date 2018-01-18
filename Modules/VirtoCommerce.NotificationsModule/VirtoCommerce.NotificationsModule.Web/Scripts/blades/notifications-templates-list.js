angular.module('virtoCommerce.notificationsModule')
.controller('virtoCommerce.notificationsModule.notificationTemplatesListController', ['$scope', 'virtoCommerce.notificationsModule.notificationsService', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.notifications', 'platformWebApp.settings', 'platformWebApp.ui-grid.extension', 'platformWebApp.uiGridHelper',
    function ($scope, notificationsService, bladeNavigationService, dialogService, notifications, settings, gridOptionExtension, uiGridHelper) {
        $scope.uiGridConstants = uiGridHelper.uiGridConstants;
	    var blade = $scope.blade;
	    blade.selectedLanguage = null;

	    if (!blade.languages) {
        var languages = [{"language":"default"}, {"language":"en-US"}, {"language":"de-DE"}];
        blade.languages = languages;
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
			    title: 'platform.blades.notifications-edit-template.title',
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

	    function createTemplate(template) {
		    var newBlade = {
			    id: 'editTemplate',
			    title: 'platform.blades.notifications-edit-template.title-new',
			    notificationType: blade.notificationType,
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
      $scope.setGridOptions = function (gridId, gridOptions) {
          $scope.gridOptions = gridOptions;
          gridOptionExtension.tryExtendGridOptions(gridId, gridOptions);

          gridOptions.onRegisterApi = function (gridApi) {
              $scope.gridApi = gridApi;
              gridApi.core.on.sortChanged($scope, function () {
                  if (!blade.isLoading) blade.refresh();
              });
          };
      };

	    blade.initialize();
}]);
