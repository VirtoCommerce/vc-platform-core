import * as signalR from '@aspnet/signalr';

angular.module('platformWebApp').config(
  ['$stateProvider', function ($stateProvider) {
    $stateProvider
      .state('workspace.pushNotificationsHistory', {
        url: '/events',
        templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
        controller: ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
          var blade = {
            id: 'events',
            title: 'platform.blades.history.title',
            breadcrumbs: [],
            subtitle: 'platform.blades.history.subtitle',
            controller: 'platformWebApp.pushNotificationsHistoryController',
            template: '$(Platform)/Scripts/app/pushNotifications/blade/history.tpl.html',
            isClosingDisabled: true
          };
          bladeNavigationService.showBlade(blade);
        }
        ]
      });
  }])
  .factory('platformWebApp.pushNotificationTemplateResolver', ['platformWebApp.bladeNavigationService', '$state', function (bladeNavigationService, $state) {
    var notificationTemplates = [];

    function register(template) {
      notificationTemplates.push(template);
      notificationTemplates.sort(function (a, b) { return a.priority - b.priority; });
    };
    function resolve(notification, place) {
      return _.find(notificationTemplates, function (x) { return x.satisfy(notification, place); });
    };
    var retVal = {
      register: register,
      resolve: resolve,
    };

    //Recent events notification template (error, info, debug)
    var menuDefaultTemplate =
    {
      priority: 1000,
      satisfy: function (notification, place) { return place == 'menu'; },
      //template for display that notification in menu and list
      template: '$(Platform)/Scripts/app/pushNotifications/menuDefault.tpl.html',
      //action executed when notification selected
      action: function (notify) { $state.go('workspace.pushNotificationsHistory', notify) }
    };

    //In history list notification template (error, info, debug)
    var historyDefaultTemplate =
    {
      priority: 1000,
      satisfy: function (notification, place) { return place == 'history'; },
      //template for display that notification in menu and list
      template: '$(Platform)/Scripts/app/pushNotifications/blade/historyDefault.tpl.html',
      //action executed in event detail
      action: function (notify) {
        var blade = {
          id: 'notifyDetail',
          title: 'platform.blades.historyDetailDefault.title',
          subtitle: 'platform.blades.historyDetailDefault.subtitle',
          template: '$(Platform)/Scripts/app/pushNotifications/blade/historyDetailDefault.tpl.html',
          isClosingDisabled: false,
          notify: notify
        };
        bladeNavigationService.showBlade(blade);
      }
    };

    retVal.register(menuDefaultTemplate);
    retVal.register(historyDefaultTemplate);

    return retVal;
  }])
  .factory('platformWebApp.pushNotificationService', ['$rootScope', '$timeout', '$interval', '$state', 'platformWebApp.mainMenuService', 'platformWebApp.pushNotificationTemplateResolver', 'platformWebApp.pushNotifications', 'platformWebApp.headerNotificationWidgetService',
    function ($rootScope, $timeout, $interval, $state, mainMenuService, eventTemplateResolver, notifications, headerNotifications) {

      //SignalR setup connection
      var connection = new signalR.HubConnectionBuilder()
        .withUrl("/pushNotificationHub")
        .build();
      connection.start();

      connection.on('Send', function (data) {
        //broadcast event
        $rootScope.$broadcast("new-notification-event", data);

        var notificationTemplate = eventTemplateResolver.resolve(data, 'menu');
        data.action = notificationTemplate.action;
        headerNotifications.addNotification(data);
      });

      var retVal = {};
      return retVal;

    }]);
