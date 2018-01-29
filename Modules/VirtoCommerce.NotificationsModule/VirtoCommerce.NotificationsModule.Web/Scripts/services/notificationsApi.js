var fakeNotifications = {
    "totalCount": 13,
    "results":
    [
        {
            "id" : "1",
            "displayName": "notifications.types.RegistrationEmailNotification.displayName",
            "description": "notifications.types.RegistrationEmailNotification.description",
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
            "notificationType": "OrderSentEmailNotification",
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
            "notificationType": "CancelOrderEmailNotification",
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
var fakeTemplates = [
    {   
        "id":"1",
        "notificationType":"RegistrationEmailNotification",
        "language":"en-US",
        "isDefault":false,
        "created" :"2018-01-01",
        "modified":"2018-01-01", 
        "displayName" :"Registration notification",
        "sendGatewayType" : "Email",
        "ccRecipients": null,
        "bccRecipients": null,
        "recipient": "a@a.com",
        "sender": "s@s.s",
        "subject": "some",
        "sendGatewayType": "Email",
        "body": "Thank you for registration {{firstname}} {{lastname}}",
        "dynamicProperties" : "{ \"firstname\": \"Name\", \"lastname\": \"Last\" }"
    }];

angular.module('virtoCommerce.notificationsModule')
  .factory('virtoCommerce.notificationsModule.notificationsService', ['$q', '$filter', function ($q, $filter) {
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
      self.getNotificationList = function(searchCreteria) {
          return fakeHttpCall(true).then(
              function(data) {
                  // success callback
                  if (searchCreteria.searchPhrase) {
                    var keyword = searchCreteria.searchPhrase.toUpperCase();  
                    var filterNotifications = _.filter(fakeNotifications.results, function(item){ 
                        return item.notificationType.toUpperCase().indexOf(keyword) !== -1; 
                    }); 
                    var result = _.clone(fakeNotifications);
                    result.results = filterNotifications;
                    result.totalCount = filterNotifications.length;  

                    return result;
                       
                  }
                  
                  return fakeNotifications;
              },
              function(err) {
                  // error callback
                  console.log(err)
              });
      }

      self.getNotificationByType = function (item) {
        return fakeHttpCall(true).then(
            function(data) {
                // success callback
                var found;
                for (var i = 0; i < fakeNotifications.results.length; i++) {
                    if (fakeNotifications.results[i].notificationType === item.type) {
                        found = fakeNotifications.results[i];
                        break;
                    }
                }
                return found;
            },
            function(err) {
                // error callback
                console.log(err)
            });
      }

      self.getTemplates = function (notification) {
        return fakeHttpCall(true).then(
            function(data) {
                // success callback
                var filterTemplates = _.filter(fakeTemplates, { 'notificationType': notification.notificationType });
                return filterTemplates;
            },
            function(err) {
                // error callback
                console.log(err)
            });
      }
      
      self.getTemplateById = function (id) {
        return fakeHttpCall(true).then(
            function(data) {
                // success callback
                var found;
                for (var i = 0; i < fakeTemplates.length; i++) {
                    if (fakeTemplates[i].id === id) {
                        found = fakeTemplates[i];
                        break;
                    }
                }
                return found;
            },
            function(err) {
                // error callback
                console.log(err)
            });
      }
      
      self.updateTemplate = function(template) {
          return fakeHttpCall(true).then(
            function(data) {
                // success callback
                template.id = (fakeTemplates.length + 1).toString();
                var date = new Date();
                template.created = date.getFullYear() + '-' + ('0' + (date.getMonth() + 1)).slice(-2) + '-' + ('0' + date.getDate()).slice(-2);
                fakeTemplates.push(template);
                return template;
            },
            function(err) {
                // error callback
                console.log(err)
            });
      }

  }

  return new notificationsService();
}]);
