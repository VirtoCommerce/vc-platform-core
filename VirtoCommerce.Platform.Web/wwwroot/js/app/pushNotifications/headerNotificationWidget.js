angular.module('platformWebApp').directive('vaHeaderNotificationWidget', ["$document",
  function ($document) {

    return {
      restrict: 'E',
      replace: true,
      transclude: true,
      templateUrl: '$(Platform)/Scripts/app/pushNotifications/headerNotificationWidget.tpl.html',
      link: function (scope, element, attr, ngModelController, linker) {

        scope.noticeOpened = false;

        function handleClickEvent(event) {
          console.log(scope.noticeOpened);
          var dropdownElement = $document.find('[notificationWidget]');
          var hadDropdownElement = $document.find('[notificationWidget] .header__nav-item--opened');
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

        scope.close = function () {
          scope.noticeOpened = false;
        };

      }
    }
  }]);
