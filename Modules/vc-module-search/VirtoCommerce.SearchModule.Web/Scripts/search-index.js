var moduleTemplateName = "virtoCommerce.searchModule";

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleTemplateName);
}

angular.module(moduleTemplateName, [])
    .config(['$stateProvider', function ($stateProvider) {
        $stateProvider.state('workspace.searchIndexModule', {
            url: '/searchIndex',
            controller: ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
                var blade = {
                    id: 'searchIndex',
                    title: 'search.blades.document-type-list.title',
                    headIcon: 'fa fa-search',
                    controller: 'virtoCommerce.searchModule.indexesListController',
                    template: 'Modules/$(VirtoCommerce.Search)/Scripts/blades/indexes-list.tpl.html',
                    isClosingDisabled: true
                };
                bladeNavigationService.showBlade(blade);
                $scope.moduleName = 'vc-core vc-search-index';
            }],
            templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html'
        });
    }])
    .run(['$rootScope', 'platformWebApp.mainMenuService', 'platformWebApp.widgetService', '$state', 'platformWebApp.pushNotificationTemplateResolver', 'platformWebApp.bladeNavigationService', function ($rootScope, mainMenuService, widgetService, $state, pushNotificationTemplateResolver, bladeNavigationService) {

        // register notification template
        pushNotificationTemplateResolver.register({
            priority: 900,
            satisfy: function (notify, place) { return place == 'history' && notify.notifyType == 'IndexProgressPushNotification'; },
            template: 'Modules/$(VirtoCommerce.Search)/Scripts/notifications/historyIndex.tpl.html',
            action: function (notify) {
                var blade = {
                    id: 'indexProgress',
                    notification: notify,
                    controller: 'virtoCommerce.searchModule.indexProgressController',
                    template: 'Modules/$(VirtoCommerce.Search)/Scripts/blades/index-progress.tpl.html'
                };
                bladeNavigationService.showBlade(blade);
            }
        });

        pushNotificationTemplateResolver.register({
            priority: 900,
            satisfy: function (notify, place) { return place == 'header-notification' && notify.notifyType == 'IndexProgressPushNotification'; },
            template: 'Modules/$(VirtoCommerce.Search)/Scripts/notifications/headerNotification.tpl.html',
            action: function (notify) { $state.go('workspace.pushNotificationsHistory', notify) }
        });

        mainMenuService.addMenuItem({
            path: 'browse/searchIndex',
            icon: 'fa fa-search',
            title: 'search.main-menu-title.search-index',
            priority: 25,
            action: function () {
                $state.go('workspace.searchIndexModule');
            }
        });
    }
    ]);
