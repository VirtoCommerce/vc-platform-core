angular.module('platformWebApp')
  .directive('vaHeaderNotificationWidget', ["$document",
    function ($document) {

      return {
        restrict: 'E',
        replace: true,
        transclude: true,
        templateUrl: '$(Platform)/Scripts/app/pushNotifications/headerNotificationWidget.tpl.html',
        link: function (scope, element, attr, ngModelController, linker) {

          scope.noticeOpened = false;

          function handleClickEvent(event) {
            var dropdownElement = $document.find('.notification-widget .header__nav-item');
            var hadDropdownElement = $document.find('.notification-widget .header__nav-item .header__nav-item--opened');
            if (scope.noticeOpened && !(dropdownElement.is(event.target) || dropdownElement.has(event.target).length > 0 ||
              hadDropdownElement.is(event.target) || hadDropdownElement.has(event.target).length > 0)) {
              scope.$apply(function () {
                scope.noticeOpened = false;
              });
            }
            console.log(scope.noticeOpened);
          }

          $document.bind('click', handleClickEvent);

          scope.$on('$destroy', function () {
            $document.unbind('click', handleClickEvent);
          });

          scope.toggleNotice = function () {
            scope.noticeOpened = !scope.noticeOpened;
          };

        }
      }
    }]);
