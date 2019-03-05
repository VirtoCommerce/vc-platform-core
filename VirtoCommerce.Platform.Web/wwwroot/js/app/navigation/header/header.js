angular.module('platformWebApp')
    .directive('vaHeader', ["$document",
        function ($document) {

            return {
                restrict: 'E',
                replace: true,
                transclude: true,
                templateUrl: '$(Platform)/Scripts/app/navigation/header/header.tpl.html',
                link: function (scope, element, attr, ngModelController, linker) {



                }
            }
        }]);
