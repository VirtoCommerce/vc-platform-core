var fakeData = {
    "totalCount": 13,
    "results":
    [
        {
            "displayName": "Registration notification",
            "description": "This notification is sent by email to a client when he finishes registration",
            "type": 'Email',
            "name": "RegistrationEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "displayName": "Reset password notification",
            "description": "This notification is sent by email to a client upon reset password request",
            "type": 'SMS',
            "name": "ResetPasswordEmailNotification",
            "isActive": false,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "displayName": "Two factor authentication",
            "description": "This notification contains a security token for two factor authentication",
            "type": 'Email',
            "name": "TwoFactorEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "displayName": "Two factor authentication",
            "description": "This notification contains a security token for two factor authentication",
            "type": 'Email',
            "name": "TwoFactorSmsNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "displayName": "Sending custom form from storefront",
            "description": "This notification sends by email to client when he complite some form on storefront, for example contact us form",
            "type": 'Email',
            "name": "StoreDynamicEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "displayName": "Create order notification",
            "description": "This notification sends by email to client when he create order",
            "type": 'Email',
            "name": "OrderCreateEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "displayName": "Order paid notification",
            "description": "This notification sends by email to client when all payments of order has status paid",
            "type": 'Email',
            "name": "OrderPaidEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "displayName": "Order sent notification",
            "description": "This notification sends by email to client when all shipments gets status sent",
            "type": 'Email',
            "name": "OrderSentEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "displayName": "New order status notification",
            "description": "This notification sends by email to client when status of orders has been changed",
            "type": 'Email',
            "name": "NewOrderStatusEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "displayName": "Cancel order notification",
            "description": "This notification sends by email to client when order canceled",
            "type": 'Email',
            "name": "CancelOrderEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "displayName": "The invoice for customer order",
            "description": "The template for for customer order invoice (used for PDF generation)",
            "type": 'Email',
            "name": "InvoiceEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "displayName": "New subscription notification",
            "description": "This notification sends by email to client when created new subscription",
            "type": 'Email',
            "name": "NewSubscriptionEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "displayName": "Subscription canceled notification",
            "description": "This notification sends by email to client when subscription was canceled",
            "type": 'Email',
            "name": "SubscriptionCanceledEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        }
    ]
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
  .factory('notificationsService', ['$q', function ($q) {
    function notificationsService(searchCriteria) {
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
            return fakeHttpCall(true).then(
                function(data) {
                    // success callback
                    return data;
                },
                function(err) {
                    // error callback
                    console.log(err)
                });
        }
    }
    return new notificationsService();
  }]);
