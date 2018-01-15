//Call this to register our module to main application
var moduleTemplateName = "virtoCommerce.notificationsModule";

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleTemplateName);
}

angular.module(moduleTemplateName, [])
    .config(['$stateProvider', '$urlRouterProvider',
        function ($stateProvider, $urlRouterProvider) {
            $stateProvider
                .state('workspace.notificationsModule', {
                    url: '/notifications',
                    templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
                    controller: [
                        '$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
                            var newBlade = {
                                id: 'notifications',
                                controller: 'virtoCommerce.notificationsModule.notificationsController',
                                template: 'Modules/$(virtoCommerce.Notifications)/Scripts/blades/notifications-list.tpl.html',
                                isClosingDisabled: true
                            };
                            bladeNavigationService.showBlade(newBlade);
                            $scope.moduleName = "vc-notifications";
                        }
                    ]
                });
        }
    ])
    .run(['$rootScope', 'platformWebApp.mainMenuService', 'platformWebApp.widgetService', '$state',
        function ($rootScope, mainMenuService, widgetService, $state) {
            //Register module in main menu
            var menuItem = {
                path: 'browse/notificationsModule',
                icon: 'fa fa-envelope',
                title: 'Notifications Module',
                priority: 7,
                action: function () { $state.go('workspace.notificationsModule'); },
                permission: 'notificationsModulePermission'
            };
            mainMenuService.addMenuItem(menuItem);
        }
    ]);


