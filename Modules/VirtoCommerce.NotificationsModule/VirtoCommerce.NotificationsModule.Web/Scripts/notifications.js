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
                    url: '/notifications?objectId&objectTypeId',
                    templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
                    controller: ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
                            var blade = {
                                id: 'notifications',
                                title: 'platform.menu.notifications',
                                subtitle: 'platform.blades.notifications-menu.subtitle',
                                controller: 'virtoCommerce.notificationsModule.notificationsMenuController',
                                template: 'Modules/$(virtoCommerce.notificationsModule)/Scripts/blades/notifications-menu.tpl.html',
                                isClosingDisabled: true
                            };
                            bladeNavigationService.showBlade(blade);
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


