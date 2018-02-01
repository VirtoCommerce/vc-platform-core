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
        "body": "Thank you for registration {{firstname}} {{lastname}}",
        "dynamicProperties" : "{\n \"firstname\": \"Name\",\n \"lastname\": \"Last\"\n}"
    }];
var fakeJournal = {"notifications":[{"id":"5c88b339b9c84bcbbb5bbc483ddc1f48","displayName":"New subscription notification","description":"This notification sends by email to client when created new subscription","isEmail":true,"isSms":false,"type":"NewSubscriptionEmailNotification","isActive":true,"isSuccessSend":false,"objectTypeId":"Subscription","language":"en-US","sendingGateway":"DefaultSmtpEmailNotificationSendingGateway","subject":"You are successfully subscribed to new subscription <strong> SU180131-00001 </strong>","body":"Subscription SU180131-00001 with amount 290.94USD successfully created.","sender":"noreply@mail.com","recipient":"test@ke.com","attemptCount":1,"maxAttemptCount":10,"lastFailAttemptMessage":"The SMTP host was not specified.","lastFailAttemptDate":"2018-01-31T07:49:01.037Z"},{"id":"277fbec3db7247e5be5c206ff8f96be8","displayName":"Registration notification","description":"This notification is sent by email to a client when he finishes registration","isEmail":true,"isSms":false,"type":"RegistrationEmailNotification","isActive":true,"isSuccessSend":true,"objectId":"Electronics","objectTypeId":"Store","language":"en-US","sendingGateway":"DefaultSmtpEmailNotificationSendingGateway","subject":"Your login - test.","body":"Thank you for registration  !!!","sender":"noreply@mail.com","recipient":"test@ke.com","attemptCount":1,"maxAttemptCount":10,"lastFailAttemptMessage":"The SMTP host was not specified.","lastFailAttemptDate":"2018-01-31T07:46:00.75Z"}],"totalCount":2};

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
                  var result = _.clone(fakeNotifications);
                  if (searchCreteria.searchPhrase) {
                    var keyword = searchCreteria.searchPhrase.toUpperCase();  
                    var filterNotifications = _.filter(fakeNotifications.results, function(item){ 
                        return item.notificationType.toUpperCase().indexOf(keyword) !== -1; 
                    }); 
                    result.results = filterNotifications;
                    result.totalCount = filterNotifications.length;  
                  }
                  if (searchCreteria.sort) {
                      //sort : "displayName:desc"
                      var sortFields = searchCreteria.sort.split(':');
                      result.results = _.sortBy(result.results, sortFields[0], sortFields[1]);
                      if (sortFields[1] === 'desc') {
                          result.results = result.results.reverse();
                      }
                  }
                  
                  
                  return result;
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
                return _.findWhere(fakeNotifications.results, { notificationType: item.type });
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
                return _.findWhere(fakeTemplates, { id: id });
            },
            function(err) {
                // error callback
                console.log(err)
            });
      }
      
      self.updateTemplate = function(template) {
          return fakeHttpCall(true).then(
            function(data) {
                var date = new Date();
                var now = date.getFullYear() + '-' + ('0' + (date.getMonth() + 1)).slice(-2) + '-' + ('0' + date.getDate()).slice(-2);
                if (template.id) {
                    template.modified = now;
                    return template; 
                }
                template.id = (fakeTemplates.length + 1).toString();
                template.created = now;
                fakeTemplates.push(template);
                return template;
            },
            function(err) {
                // error callback
                console.log(err)
            });
      }
      
      self.getNotificationJournalList = function(searchCriteria) {
          return fakeHttpCall(true).then(
            function(data) {
                return fakeJournal;
            },
            function(err) {
                // error callback
                console.log(err)
            });
      }

  }

  return new notificationsService();
}]);
