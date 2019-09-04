angular.module('virtoCommerce.catalogModule').directive('vaProperty2', ['$compile', '$filter', '$parse', '$templateCache', '$http', function ($compile, $filter, $parse, $templateCache, $http) {

    return {
        restrict: 'E',
        require: ['^form', 'ngModel'],
        replace: true,
        transclude: true,
        templateUrl: 'Modules/$(VirtoCommerce.Catalog)/Scripts/directives/property2.tpl.html',
        scope: {
            languages: "=",
            hiddenLanguages: "=",
            defaultLanguage: "=",
            getPropValues: "&",
            pageSize: "@?"
        },
        link: function (scope, element, attr, ctrls, linker) {
            var ngModelController = ctrls[1];

            scope.currentEntity = ngModelController.$modelValue;

            scope.context = {};
            scope.context.currentPropValues = [];
            scope.context.allDictionaryValues = [];
            scope.context.langValuesMap = {};
            scope.context.form = ctrls[0];

            scope.pageSize = angular.isDefined(scope.pageSize) ? scope.pageSize : 50;

            scope.$watch('context.langValuesMap', function (newValue, oldValue) {
                if (newValue != oldValue && !scope.currentEntity.dictionary) {
                    scope.context.currentPropValues = [];
                    angular.forEach(scope.context.langValuesMap, function (langGroup, languageCode) {
                        angular.forEach(langGroup.currentPropValues, function (propValue) {
                            propValue.languageCode = languageCode;
                            scope.context.currentPropValues.push(propValue);
                        });
                    });
                }
            }, true);

            scope.$watch('context.currentPropValues', function (newValues) {
                //reflect only real changes
                var currentValues = angular.copy(scope.currentEntity.values);

                if (scope.currentEntity.dictionary) {
                    currentValues = _.uniq(_.map(currentValues, function (x) {
                        return {
                            id: x.id,
                            alias: x.alias,
                            valueId: x.valueId,
                            value: x.alias
                        };
                    }), function (x) { return x.valueId; });
                }

                if (isValuesDifferent(newValues, currentValues)) {
                    if (newValues[0] === undefined) {
                        scope.currentEntity.values = null;
                    } else {
                        scope.currentEntity.values = newValues;
                    }
                    //reset inherited status to force property value override
                    _.each(scope.currentEntity.values, function (x) { x.isInherited = false; });

                    ngModelController.$setViewValue(scope.currentEntity);
                }
                if (newValues[0] === undefined) {
                    scope.currentEntity.values = [];
                }
            }, true);


            ngModelController.$render = function () {
                scope.currentEntity = ngModelController.$modelValue;

                scope.context.currentPropValues = angular.copy(scope.currentEntity.values);
                //For dictionary multilingual properties need to left only distinct dictionary items
                if (scope.currentEntity.dictionary) {
                    scope.context.currentPropValues = _.uniq(_.map(scope.context.currentPropValues, function (x) {
                        return {
                            id: x.id,
                            alias: x.alias,
                            valueId: x.valueId,
                            value: x.alias,
                            selected: true
                        };
                    }), function (x) { return x.valueId; });
                }
                if (needAddEmptyValue(scope.currentEntity, scope.context.currentPropValues)) {
                    scope.context.currentPropValues.push({ value: null });
                }

                initLanguagesValuesMap();

                chageValueTemplate(scope.currentEntity.valueType);
            };

            function isValuesDifferent(newValues, currentValues) {
                var elementCountIsDifferent = newValues.length != currentValues.length;
                var elementsNotEqual = _.any(newValues, function (newValue) {
                    return _.all(currentValues, function (currentValue) {
                        return !(newValue && currentValue.value === newValue.value && currentValue.languageCode == newValue.languageCode);
                    });
                });

                return (elementCountIsDifferent || elementsNotEqual) &&
                    (_.any(currentValues) || (newValues[0] && newValues[0].value)); //Prevent reflecting the change when null value was added to empty initial values
            };

            function needAddEmptyValue(property, values) {
                return !property.multivalue && !property.dictionary && values.length == 0;
            };


            function initLanguagesValuesMap() {
                if (scope.currentEntity.multilanguage && !scope.currentEntity.dictionary) {
                    //Group values by language 
                    angular.forEach(scope.languages, function (language) {
                        //Currently select values
                        var currentPropValues = _.where(scope.context.currentPropValues, { languageCode: language });
                        // provide default value if value wasn't found in specified language.
                        if (!_.any(currentPropValues) && _.any(scope.context.currentPropValues)) {
                            currentPropValues = angular.copy(_.filter(scope.context.currentPropValues, function (x) { return !x.languageCode }));
                            _.each(currentPropValues, function (x) {
                                x.id = null;
                                x.languageCode = language;
                            });
                        }
                        //need add empty value for single  value type
                        if (needAddEmptyValue(scope.currentEntity, currentPropValues)) {
                            currentPropValues.push({ value: null, languageCode: language });
                        }
                        //All possible dict values
                        var allValues = _.where(scope.context.allDictionaryValues, { languageCode: language });

                        var langValuesGroup = {
                            allValues: allValues,
                            currentPropValues: currentPropValues
                        };
                        scope.context.langValuesMap[language] = langValuesGroup;
                    });
                }
            };

            scope.isLanguageVisible = function (language) {
                if (scope.hiddenLanguages) {
                    if (_.contains(scope.hiddenLanguages, language)) {
                        return false;
                    }
                }

                return true;
            }

            scope.loadDictionaryValues = function ($select) {
                $select.page = 0;
                scope.context.allDictionaryValues = [];
                return scope.loadNextDictionaryValues($select);
            };

            scope.loadNextDictionaryValues = function ($select) {
                var countToSkip = $select.page * scope.pageSize;
                var countToTake = scope.pageSize;

                return scope.getPropValues()(scope.currentEntity.id, $select.search, countToSkip, countToTake).then(function (result) {
                    populateDictionaryValues(result.results, scope.currentEntity.name);
                    $select.page++;
                    // If there are more items to display, let's prepare to handle these items.
                    if (scope.context.allDictionaryValues.length < result.totalCount) {
                        // Reset scrolling for the when-scrolled directive, so it could trigger this method for next page.
                        scope.$broadcast('scrollCompleted');
                    }

                    return result;
                });
            }

            function populateDictionaryValues(dictItems, propertyName) {
                angular.forEach(dictItems, function (dictItem) {
                    var dictValue = _.find(scope.context.currentPropValues, function (x) {
                        return x && (x.valueId == dictItem.id);
                    });
                    if (!dictValue) {
                        dictValue = {
                            alias: dictItem.alias,
                            valueId: dictItem.id,
                            value: dictItem.alias
                        };
                    }
                    scope.context.allDictionaryValues.push(dictValue);
                });
            }

            function getTemplateName(property) {
                var result = property.valueType;

                if (property.dictionary) {
                    result += '-dictionary';
                }
                if (property.multivalue) {
                    result += '-multivalue';
                }
                if (property.multilanguage) {
                    result += '-multilang';
                }
                result += '.html';
                return result;
            };

            function chageValueTemplate(valueType) {
                var templateName = getTemplateName(scope.currentEntity);


                //load input template and display
                $http.get(templateName, { cache: $templateCache }).then(function (results) {
                    //Need to add ngForm to isolate form validation into sub form
                    //var innerContainer = "<div id='innerContainer' />";

                    //We must destroy scope of elements we are removing from DOM to avoid angular firing events
                    var el = element.find('#valuePlaceHolder #innerContainer');
                    if (el.length > 0) {
                        el.scope().$destroy();
                    }
                    var container = element.find('#valuePlaceHolder');
                    var result = container.html(results.data.trim());

                    //Create new scope, otherwise we would destroy our directive scope
                    var newScope = scope.$new();
                    $compile(result)(newScope);
                });
            };

            /* Datepicker */
            scope.datepickers = {
                DateTime: false
            }
            scope.open = function ($event, which) {
                $event.preventDefault();
                $event.stopPropagation();

                scope.datepickers[which] = true;
            };

            scope.dateOptions = {
                datepickerMode: 'day'
            };

            linker(function (clone) {
                element.append(clone);
            });

            function setValid(name) {
                var form = scope.context.form;
                form[name].$setValidity('minlength', true);
                form[name].$setValidity('maxlength', true);
                form[name].$setValidity('pattern', true);
            }

            scope.tagsDeleted = function (tag, name, required) {
                var form = scope.context.form;
                var values = scope.context.currentPropValues;
                if (required && values.length === 0) {
                    form[name].$setValidity('required', false);
                }

                setValid(name);
            };

            scope.tagsAdded = function (tag, name) {
                var form = scope.context.form;
                if (tag.value) {
                    form[name].$setValidity('required', true);
                }

                setValid(name);
            };

            scope.addederror = function (tag, name, minValue, maxValue, pattern) {
                var form = scope.context.form;
                if (minValue && tag.value.length < minValue) {
                    form[name].$setValidity('minlength', false);
                }

                if (maxValue && tag.value.length > maxValue) {
                    form[name].$setValidity('maxlength', false);
                }

                if (pattern) {
                    var re = new RegExp(pattern);
                    if (!re.test(tag.value)) {
                        form[name].$setValidity('pattern', false);
                    }
                }
            };
        }
    }
}]);
