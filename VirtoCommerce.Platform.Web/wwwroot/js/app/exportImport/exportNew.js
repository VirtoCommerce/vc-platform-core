angular.module('platformWebApp')
    .config(['$stateProvider', function ($stateProvider) {
        $stateProvider.state('workspace.exportNew', {
            url: '/exportNew',
            templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
            controller: ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
                var blade = {
                    id: 'exportNew',
                    controller: 'virtoCommerce.exportModule.exportGroupsController',
                    template: 'Modules/$(VirtoCommerce.Export)/Scripts/blades/exportGroups.tpl.html',
                    isClosingDisabled: true
                };
                bladeNavigationService.showBlade(blade);
            }]
        });
    }])
    .run(
        ['$rootScope', 'platformWebApp.mainMenuService', 'platformWebApp.widgetService', '$state', 'platformWebApp.pushNotificationTemplateResolver', 'platformWebApp.bladeNavigationService', 'platformWebApp.exportImport.resource', 'platformWebApp.setupWizard', function ($rootScope, mainMenuService, widgetService, $state, pushNotificationTemplateResolver, bladeNavigationService, exportImportResourse, setupWizard) {
            var menuItem = {
                path: 'configuration/exportNew',
                icon: 'fa fa-database',
                title: 'platform.menu.export-new',
                priority: 10,
                action: function () { $state.go('workspace.exportNew'); },
                permission: 'platform:exportImport:access'
            };
            mainMenuService.addMenuItem(menuItem);
        }]);
