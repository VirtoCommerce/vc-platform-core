angular.module('virtoCommerce.catalogModule')
.directive('vaPropertyMessages', function () {
    return {
        restrict: 'E',
        templateUrl: 'Modules/$(VirtoCommerce.Catalog)/Scripts/directives/propertyMessages.tpl.html',
        scope: false
    }
});