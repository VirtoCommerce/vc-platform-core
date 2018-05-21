angular.module('virtoCommerce.catalogModule')
.controller('virtoCommerce.catalogModule.imageDetailsController',
    ['$scope', '$filter', '$translate', 'platformWebApp.bladeNavigationService', 'platformWebApp.assets.api', 'platformWebApp.settings',
        function ($scope, $filter, $translate, bladeNavigationService, assets, settings) {
            var blade = $scope.blade;

            blade.currentEntity = angular.copy(blade.origEntity);
            blade.title = blade.currentEntity.name;

            blade.isLoading = false;
            $scope.isValid = true;

            var promise = settings.getValues({ id: 'VirtoCommerce.Core.General.Languages' }).$promise;
            $scope.languages = [];

            function initialize() {
                promise.then(function(promiseData) {
                    $scope.languages = promiseData;
                });
                $scope.imageTypes = settings.getValues({ id: 'Catalog.ImageCategories' });
            };

            var formScope;
            $scope.setForm = function(form) {
                 formScope = form;
            }

            $scope.$watch("blade.currentEntity", function (newValue, oldValue) {
                if (newValue !== oldValue) {
                    $scope.isValid = formScope && formScope.$valid;
                }
            }, true);

            $scope.openDictionarySettingManagement = function () {
                var newBlade = {
                    id: 'settingDetailChild',
                    isApiSave: true,
                    currentEntityId: 'Catalog.ImageCategories',
                    parentRefresh: function (data) { $scope.imageTypes = data; },
                    controller: 'platformWebApp.settingDictionaryController',
                    template: '$(Platform)/Scripts/app/settings/blades/setting-dictionary.tpl.html'
                };
                bladeNavigationService.showBlade(newBlade, blade);
            };

            $scope.saveChanges = function () {
                angular.copy(blade.currentEntity, blade.origEntity);
                $scope.bladeClose();
            }

            initialize();
        }]);
