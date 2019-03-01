angular.module('virtoCommerce.catalogModule')
.directive('vcItemSearch', ['$localStorage', 'platformWebApp.bladeNavigationService', 'virtoCommerce.catalogModule.predefinedSearchFilters', function ($localStorage, bladeNavigationService, predefinedSearchFilters) {
    return {
        restrict: 'E',
        templateUrl: function (elem, attrs) {
            return attrs.templateUrl || 'catalog-itemSearch-default.html'
        },
        scope: {
            blade: '='
        },       
        link: function ($scope) {
            var blade = $scope.blade;
            $scope.$localStorage = $localStorage;
            var filter = $scope.filter = blade.filter;

            if ($localStorage.catalogSearchFilterId && !filter.keyword && filter.keyword !== null) {
                filter.current = _.findWhere($localStorage.catalogSearchFilters, { id: $localStorage.catalogSearchFilterId });
                filter.keyword = filter.current ? filter.current.keyword : '';
                filter.searchInVariations = filter.current ? filter.current.searchInVariations : false;             
            }

            filter.change = function (isDetailBladeOpen) {
                $localStorage.catalogSearchFilterId = filter.current ? filter.current.id : null;
                if (filter.current && !filter.current.id) {
                    filter.current = null;
                    showFilterDetailBlade({ isNew: true });
                } else {
                    if (!isDetailBladeOpen)
                        bladeNavigationService.closeBlade({ id: 'filterDetail' });
                    filter.keyword = filter.current ? filter.current.keyword : '';
                    filter.searchInVariations = filter.current ? filter.current.searchInVariations : false;
                    filter.criteriaChanged();
                }
            };

            filter.edit = function ($event, entry) {
                filter.current = entry;
                showFilterDetailBlade({ data: entry });
            };

            function showFilterDetailBlade(bladeData) {
                var newBlade = {
                    id: 'filterDetail',
                    controller: 'virtoCommerce.catalogModule.filterDetailController',
                    template: 'Modules/$(VirtoCommerce.Catalog)/Scripts/blades/filter-detail.tpl.html',
                };
                angular.extend(newBlade, bladeData);
                bladeNavigationService.showBlade(newBlade, blade);
            };

            filter.criteriaChanged();
        }
    }
}]);