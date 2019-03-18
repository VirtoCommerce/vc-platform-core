var moduleName = "virtoCommerce.imageToolsModule";

if (AppDependencies != undefined) {
    AppDependencies.push(moduleName);
}
angular.module(moduleName, ['ui.grid.infiniteScroll'])
    .constant('imageToolsConfig',
        {
            intMaxValue: 2147483647
        })
    .config(['$stateProvider', function ($stateProvider) {
        $stateProvider
            .state('workspace.thumbnail', {
                url: '/thumbnail',
                templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
                controller: ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
                    var blade = {
                        id: 'thumbnailList',
                        title: 'imageTools.blades.tasks-list.title',
                        subtitle: 'imageTools.blades.tasks-list.subtitle',
                        controller: 'virtoCommerce.imageToolsModule.taskListController',
                        template: 'Modules/$(VirtoCommerce.ImageTools)/Scripts/blades/task-list.tpl.html',
                        isClosingDisabled: true
                    };
                    bladeNavigationService.showBlade(blade);
                }]
            });
    }])
    .run(
        ['$rootScope', 'platformWebApp.mainMenuService', 'platformWebApp.widgetService', '$state', 'platformWebApp.authService', 'platformWebApp.pushNotificationTemplateResolver', 'platformWebApp.bladeNavigationService', function ($rootScope, mainMenuService, widgetService, $state, authService, pushNotificationTemplateResolver, bladeNavigationService) {
            var menuItem = {
                path: 'browse/thumbnail',
                icon: 'fa fa-picture-o',
                title: 'imageTools.main-menu-title',
                priority: 901,
                action: function () { $state.go('workspace.thumbnail'); },
                permission: 'thumbnail:access'
            };
            mainMenuService.addMenuItem(menuItem);

            var menuTemplate =
            {
                priority: 901,
                satisfy: function (notify, place) { return place == 'header-notification' && notify.notifyType == 'ThumbnailProcess'; },
                template: 'Modules/$(VirtoCommerce.ImageTools)/Scripts/notifications/headerNotification.tpl.html',
                action: function (notify) { $state.go('workspace.pushNotificationsHistory', notify) }
            };
            pushNotificationTemplateResolver.register(menuTemplate);

            var historyTemplate =
            {
                priority: 901,
                satisfy: function (notify, place) { return place == 'history' && notify.notifyType == 'ThumbnailProcess'; },
                template: 'Modules/$(VirtoCommerce.ImageTools)/Scripts/notifications/historyThumbnailProcess.tpl.html',
                action: function (notify) {
                    var blade = {
                        id: 'thumbnailProcessDetail',
                        title: 'Title1',
                        subtitle: 'Subtite1',
                        template: 'Modules/$(VirtoCommerce.ImageTools)/Scripts/blades/task-progress.tpl.html',
                        controller: 'virtoCommerce.imageToolsModule.taskRunController',
                        notification: notify
                    };
                    bladeNavigationService.showBlade(blade);
                }
            };
            pushNotificationTemplateResolver.register(historyTemplate);
        }]);
