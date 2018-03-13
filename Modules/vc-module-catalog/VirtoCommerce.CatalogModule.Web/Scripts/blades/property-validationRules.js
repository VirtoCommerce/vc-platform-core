angular.module('virtoCommerce.catalogModule')
    .controller('virtoCommerce.catalogModule.propertyValidationRulesController', ['$scope', function ($scope) {
        var parentEntity = $scope.blade.parentBlade.currentEntity;
        var rule = $scope.blade.parentBlade.currentEntity.validationRule;
        if (!rule) rule = {};

        $scope.blade.propertyValidationRule = {
            id: rule.id,
            required: parentEntity.required,
            isLimited: rule.charCountMin != null || rule.charCountMax != null,
            isSpecificPattern: rule.regExp != null,
            charCountMin: rule.charCountMin,
            charCountMax: rule.charCountMax,
            selectedLimit: 'between',
            characterLimit: ['between', 'at-least', 'not-more-than'],
            validationPatterns: [
                {
                    name: 'custom',
                    pattern: ""
                },
                {
                    name: "email",
                    pattern: "^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$"
                },
                {
                    name: "url",
                    pattern: "https?:\\/\\/(www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{2,256}\\.[a-z]{2,6}\\b([-a-zA-Z0-9@:%_\\+.~#?&//=]*)"
                },
                {
                    name: "date",
                    pattern: "(0[1-9]|[12][0-9]|3[01])[ \.-](0[1-9]|1[012])[ \.-](19|20|)\d\d"
                }
            ],
            selectedPattern: { name: 'custom', pattern: rule.regExp },
            pattern: rule.regExp
        };

        if (rule.charCountMin && !rule.charCountMax)
            $scope.blade.propertyValidationRule.selectedLimit = 'at-least';
        else if (!rule.charCountMin && rule.charCountMax)
            $scope.blade.propertyValidationRule.selectedLimit = 'not-more-than';
        else if (rule.charCountMin && rule.charCountMax)
            $scope.blade.propertyValidationRule.selectedLimit = 'between';
        $scope.selectOption = function (option) {
            $scope.blade.property.valueType = option;
            $scope.bladeClose();
        };

        $scope.blade.headIcon = 'fa-gear';
        $scope.blade.isLoading = false;

        $scope.saveChanges = function () {
            $scope.blade.parentBlade.currentEntity.required = $scope.blade.propertyValidationRule.required;
            $scope.blade.parentBlade.currentEntity.validationRule = {
                id: $scope.blade.propertyValidationRule.id,
                charCountMin: $scope.blade.propertyValidationRule.isLimited ? $scope.blade.propertyValidationRule.charCountMin : null,
                charCountMax: $scope.blade.propertyValidationRule.isLimited ? $scope.blade.propertyValidationRule.charCountMax : null,
                regExp: $scope.blade.propertyValidationRule.isSpecificPattern ? $scope.blade.propertyValidationRule.selectedPattern.pattern : null
            };
            $scope.bladeClose();
        };

        var formScope;
        $scope.setForm = function (form) {
            formScope = form;
        };
        $scope.$watch('blade.propertyValidationRule', function () {
            $scope.isValid = formScope && formScope.$valid;
        }, true);

    }]);
