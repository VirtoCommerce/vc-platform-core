angular.module('virtoCommerce.orderModule')
    // directive mask the price if value passed to directive is true
    .directive('maskMoney', ['showPriceFilter', function (showPriceFilter) {
        return {
            restrict: 'A',
            require: 'ngModel',
            scope: {
                maskMoney: '=',
            },
            link: function (scope, el, attrs, ngModelCtrl) {
                var mask = scope.maskMoney;

                if (mask) {
                    el.attr('readonly', true);
                }

                ngModelCtrl.$formatters.unshift(function (value) {
                    return mask ? showPriceFilter(value, false) : value;
                });
            }
        };
    }]);
