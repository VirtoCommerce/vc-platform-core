angular.module('platformWebApp').directive('vaHeader', ["$state",
  function ($state) {
    return {
      restrict: 'E',
      replace: true,
      transclude: true,
      templateUrl: '$(Platform)/Scripts/app/navigation/header/header.tpl.html',
      link: function (scope) {
        scope.manageSettings = function () {
          $state.go('workspace.modulesSettings');
        }
      }
    }
  }]);
