var fakeData = {
    "totalCount": 1,
    "results":
    [{
        "id": "123",
        "displayName": "Registration notification",
        "description": "This notification is sent by email to a client when he finishes registration",
        "isEmail": true,
        "isSms": false,
        "type": "RegistrationEmailNotification",
        "isActive": true,
        "isSuccessSend": false,
        "attemptCount": 0,
        "maxAttemptCount": 10
    }]
};


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
        var fakeHttpCall = function (isSuccessful) {
            var deferred = $q.defer()
            if (isSuccessful === true) {
                deferred.resolve(fakeData)
            }
            else {
                deferred.reject("Oh no! Something went terribly wrong in you fake $http call")
            }
            return deferred.promise
        }
        self.getNotificationList = function() {
            //return fakeHttpCall(true).then(
            //    function(data) {
            //        // success callback
            //        console.log(data)
            //    },
            //    function(err) {
            //        // error callback
            //        console.log(err)
            //    });


            return $http.get("api/notification/notifications");
        }
    }
    return new notificationsService();
  }]);
