//Call this to register our module to main application
var moduleName = "virtoCommerce.exportModule";

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, ['ui.grid.cellNav', 'ui.grid.edit', 'ui.grid.validate'])
    .config(['$stateProvider', function ($stateProvider) {
        $stateProvider.state('workspace.exportModule', {
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
        ['$http', '$compile', 'platformWebApp.mainMenuService', 'platformWebApp.widgetService', '$state', 'platformWebApp.authService'
            , function ($http, $compile, mainMenuService, widgetService, $state, authService) {
                //Register module in main menu
                var menuItem = {
                    path: 'configuration/export',
                    icon: 'fa fa-database',
                    title: 'export.main-menu-title',
                    priority: 30,
                    action: function () { $state.go('workspace.exportModule'); },
                    permission: 'export:access'
                };
                mainMenuService.addMenuItem(menuItem);
            }]);
