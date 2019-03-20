angular.module('virtoCommerce.catalogModule')
    .directive('vaDictionaryValue', function () {
        return {
            restrict: 'E',
            templateUrl: 'Modules/$(VirtoCommerce.Catalog)/Scripts/directives/dictionaryValue.tpl.html',
            require: ['^form', 'ngModel'],
            scope: {
                ngModel: "=",
                name: "=",
                form: "=",
                validationRules: "=",
                required: "@"
            },
            replace: true,
            transclude: true
        }
    });
