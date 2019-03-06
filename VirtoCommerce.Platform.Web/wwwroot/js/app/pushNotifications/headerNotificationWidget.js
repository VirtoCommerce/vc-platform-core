angular.module('platformWebApp')
  .factory('platformWebApp.headerNotificationWidgetService', ['$rootScope', '$timeout', '$interval', '$state', 'platformWebApp.mainMenuService', 'platformWebApp.pushNotificationTemplateResolver', 'platformWebApp.pushNotifications',
    function ($rootScope, $timeout, $interval, $state, mainMenuService, eventTemplateResolver, notifications) {
      var notifications = [];

      var retVal = {
        notifications: []
      };

      return retVal;
    }])
  .directive('vaHeaderNotificationWidget', ["$document", "platformWebApp.headerNotificationWidgetService",
    function ($document, headerNotifications) {

      return {
        restrict: 'E',
        replace: true,
        transclude: true,
        templateUrl: '$(Platform)/Scripts/app/pushNotifications/headerNotificationWidget.tpl.html',
        link: function (scope, element, attr, ngModelController, linker) {

          scope.dropDownOpened = false;
          scope.notifications = headerNotifications.notifications;

          function handleClickEvent(event) {
            var dropdownElement = $document.find('[notificationWidget]');
            var hadDropdownElement = $document.find('[notificationWidget] .header__nav-item--opened');
            if (scope.dropDownOpened && !(dropdownElement.is(event.target) || dropdownElement.has(event.target).length ||
              hadDropdownElement.is(event.target) || hadDropdownElement.has(event.target).length)) {
              scope.$apply(function () {
                scope.dropDownOpened = false;
              });
            }
          }

          $document.bind('click', handleClickEvent);

          scope.$on('$destroy', function () {
            $document.unbind('click', handleClickEvent);
          });

          scope.toggleNotice = function () {
            scope.dropDownOpened = !scope.dropDownOpened;
          };

          scope.close = function () {
            scope.dropDownOpened = false;
          };

          scope.clearResent = function () {
            headerNotifications.notifications = [];
            scope.notifications = headerNotifications.notifications;
          }

        }
      }
    }]);
