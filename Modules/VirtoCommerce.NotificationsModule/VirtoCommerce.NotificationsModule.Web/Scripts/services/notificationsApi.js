var fakeNotifications = {
    "totalCount": 13,
    "results":
    [
        {
            "id" : "1",
            "displayName": "Registration notification",
            "description": "This notification is sent by email to a client when he finishes registration",
            "sendGatewayType": 'Email',
            "notificationType": "RegistrationEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "id" : "2",
            "displayName": "Reset password notification",
            "description": "This notification is sent by email to a client upon reset password request",
            "sendGatewayType": 'SMS',
            "notificationType": "ResetPasswordEmailNotification",
            "isActive": false,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "id" : "3",
            "displayName": "Two factor authentication",
            "description": "This notification contains a security token for two factor authentication",
            "sendGatewayType": 'Email',
            "notificationType": "TwoFactorEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "id" : "4",
            "displayName": "Two factor authentication",
            "description": "This notification contains a security token for two factor authentication",
            "sendGatewayType": 'Email',
            "notificationType": "TwoFactorSmsNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "id" : "5",
            "displayName": "Sending custom form from storefront",
            "description": "This notification sends by email to client when he complite some form on storefront, for example contact us form",
            "sendGatewayType": 'Email',
            "notificationType": "StoreDynamicEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "id" : "6",
            "displayName": "Create order notification",
            "description": "This notification sends by email to client when he create order",
            "sendGatewayType": 'Email',
            "notificationType": "OrderCreateEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "id" : "7",
            "displayName": "Order paid notification",
            "description": "This notification sends by email to client when all payments of order has status paid",
            "sendGatewayType": 'Email',
            "notificationType": "OrderPaidEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "id" : "8",
            "displayName": "Order sent notification",
            "description": "This notification sends by email to client when all shipments gets status sent",
            "sendGatewayType": 'Email',
            "type": "OrderSentEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "id" : "9",
            "displayName": "New order status notification",
            "description": "This notification sends by email to client when status of orders has been changed",
            "sendGatewayType": 'Email',
            "notificationType": "NewOrderStatusEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "id" : "10",
            "displayName": "Cancel order notification",
            "description": "This notification sends by email to client when order canceled",
            "sendGatewayType": 'Email',
            "type": "CancelOrderEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "id" : "11",
            "displayName": "The invoice for customer order",
            "description": "The template for for customer order invoice (used for PDF generation)",
            "sendGatewayType": 'Email',
            "notificationType": "InvoiceEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "id" : "12",
            "displayName": "New subscription notification",
            "description": "This notification sends by email to client when created new subscription",
            "sendGatewayType": 'Email',
            "notificationType": "NewSubscriptionEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        },
        {
            "id" : "13",
            "displayName": "Subscription canceled notification",
            "description": "This notification sends by email to client when subscription was canceled",
            "sendGatewayType": 'Email',
            "notificationType": "SubscriptionCanceledEmailNotification",
            "isActive": true,
            "isSuccessSend": false,
            "attemptCount": 0,
            "maxAttemptCount": 10
        }
    ]
};
var fakeTemplates = [{"id":"14073a3095bd42feac37bf083b832784","notificationTypeId":"RegistrationEmailNotification","language":"en-US","isDefault":false,"created" :"2018-01-01","modified":"2018-01-01"}]

angular.module('virtoCommerce.notificationsModule')
  .factory('virtoCommerce.notificationsModule.notificationsService', ['$q', function ($q) {
  function notificationsService(searchCriteria) {
      var self = this;
      var fakeHttpCall = function (isSuccessful) {
          var deferred = $q.defer()
          if (isSuccessful === true) {
              deferred.resolve("yeap!")
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
                  return fakeNotifications;
              },
              function(err) {
                  // error callback
                  console.log(err)
              });
      }

      self.getTemplates = function () {
        return fakeHttpCall(true).then(
            function(data) {
                // success callback
                return fakeTemplates;
            },
            function(err) {
                // error callback
                console.log(err)
            });
      }
  }

  return new notificationsService();
}]);
