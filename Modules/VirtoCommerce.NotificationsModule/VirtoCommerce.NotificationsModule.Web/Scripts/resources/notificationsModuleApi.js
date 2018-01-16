angular.module('virtoCommerce.notificationsModule')
    .factory('notificationsModuleApi', ['$resource', function ($resource) {
        return $resource('api/notification/notifications/:id', { id: '@Id' }, {
            getNotificationList: { method: 'GET', url: 'api/notification/notifications', isArray: true },
            //getTemplateById: { method: 'GET', url: 'api/notification/notifications/template/:id' },
            //getTemplate: { method: 'GET', url: 'api/notification/notifications/template' },
            //getTemplates: { method: 'GET', url: 'api/notification/notifications/templates', isArray: true },
            //updateTemplate: { method: 'POST', url: 'api/notification/notifications/template' },
            //deleteTemplate: { method: 'DELETE', url: 'api/notification/notifications/template/:id' },
            //prepareTestData: { method: 'GET', url: 'api/notification/notifications/template/:type/getTestingParameters', isArray: true },
            //resolveNotification: { method: 'POST', url: 'api/notification/notifications/template/rendernotificationcontent' },
            //sendNotification: { method: 'POST', url: 'api/notification/notifications/template/sendnotification' },
            //getNotificationJournalList: { method: 'GET', url: 'api/notification/notifications/journal/:objectId/:objectTypeId' },
            //getNotificationJournalDetails: { method: 'GET', url: 'api//notificationnotifications/notification/:id' },
            //stopSendingNotifications: { method: 'POST', url: 'api/notification/notifications/stopnotifications' }
        })
    }])
  .factory('notificationsService', ['$http', '$q', function ($http, $q) {
    function notificationsService() {
      var self = this;
      self.getNotificationList = function() {
          return $http.get("api/notification/notifications");
      }
    }
    return new notificationsService();
  }]);
