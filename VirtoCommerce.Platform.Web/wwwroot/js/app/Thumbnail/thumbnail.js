angular.module('platformWebApp')
    .config(['$stateProvider', function ($stateProvider) {
        $stateProvider
            .state('workspace.thumbnail', {
                url: '/thumbnail',
                templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
                controller: ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
                    var blade = {
                        id: 'thumbnailList',
                        title: 'platform.blades.thumbnail.title',
                        subtitle: 'platform.blades.thumbnail.subtitle',
                        controller: 'platformWebApp.thumbnail.taskListController',
                        template: '$(Platform)/Scripts/app/thumbnail/blades/task-list.tpl.html',
                        isClosingDisabled: true
                    };
                    bladeNavigationService.showBlade(blade);
                }]
            });
    }])
    .run(
        ['platformWebApp.mainMenuService', '$state', function (mainMenuService, $state) {
            var menuItem = {
                path: 'browse/thumbnail',
                icon: 'fa fa-picture-o',
                title: 'platform.menu.thumbnail',
                priority: 30,
                action: function () { $state.go('workspace.thumbnail'); },
                permission: 'platform:thumbnail:access'
            };
            mainMenuService.addMenuItem(menuItem);

            // ToDo register notification template

            //pushNotificationTemplateResolver.register({
            //    priority: 900,
            //    satisfy: function (notify, place) { return place == 'history' && notify.notifyType == 'IndexProgressPushNotification'; },
            //    template: '$(Platform)/Scripts/app/pushNotifications/blade/historyDefault.tpl.html',
            //    action: function (notify) {
            //        var blade = {
            //            id: 'indexProgress',
            //            notification: notify,
            //            controller: 'virtoCommerce.coreModule.indexProgressController',
            //            template: 'Modules/$(VirtoCommerce.Core)/Scripts/SearchIndex/blades/index-progress.tpl.html'
            //        };
            //        bladeNavigationService.showBlade(blade);
            //    }
            //});
        }]);
