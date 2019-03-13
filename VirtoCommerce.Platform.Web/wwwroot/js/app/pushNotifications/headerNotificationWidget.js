angular.module('platformWebApp')
  .factory('platformWebApp.headerNotificationWidgetService', ['platformWebApp.pushNotifications',
    function (notifications) {

      var notifications = [];
      var newCount = 0;

      function addNotification(notification) {
        var exitstItem = _.find(notifications, function (x) { return x.id == notification.id; });
        if (exitstItem) {
          angular.copy(notification, exitstItem);
        } else {
          notifications.push(notification);
          newCount = newCount + 1;
        }
      }

      function clear() {
        notifications = [];
        newCount = 0;
      }

      function getNotifications() {
        return notifications;
      }

      function getNewCount() {
        return newCount;
      }

      function markAllAsReaded() {
        newCount = 0;
      }

      function newAviable() {
        return newCount > 0;
      }

      var retVal = {
        addNotification: addNotification,
        getNotifications: getNotifications,
        getNewCount: getNewCount,
        newAviable: newAviable,
        markAllAsReaded: markAllAsReaded,
        clear: clear
      };

      return retVal;
    }])
  .directive('vaHeaderNotificationWidget', ['$document', 'platformWebApp.headerNotificationWidgetService', '$state',
    function ($document, headerNotifications, $state) {

      return {
        restrict: 'E',
        replace: true,
        transclude: true,
        scope: {},
        templateUrl: '$(Platform)/Scripts/app/pushNotifications/headerNotificationWidget.tpl.html',
        link: function (scope) {

          scope.dropDownOpened = false;
          scope.getNotifications = headerNotifications.getNotifications;
          scope.getNewCount = headerNotifications.getNewCount;
          scope.newAviable = headerNotifications.newAviable;

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

          scope.toggleDropDown = function () {
            scope.dropDownOpened = !scope.dropDownOpened;
            if (scope.dropDownOpened) {
              headerNotifications.markAllAsReaded();
            }
          };

          scope.close = function () {
            scope.dropDownOpened = false;
          };

          scope.clearRecent = function () {
            headerNotifications.clear();
          }

          scope.viewHistory = function () {
            scope.dropDownOpened = false;
            $state.go('workspace.pushNotificationsHistory');
          }

          scope.notificationClick = function (notification) {
            scope.dropDownOpened = false;
            notification.action(notification);
          }

        }
      }
    }]);
