angular.module('virtoCommerce.notificationsModule')
.directive('vcJournalSearch', ['$localStorage', 'platformWebApp.bladeNavigationService', 'virtoCommerce.notificationsModule.predefinedSearchFilters', function ($localStorage, bladeNavigationService, predefinedSearchFilters) {
    return {
        restrict: 'E',
        templateUrl: function (elem, attrs) {
            return attrs.templateUrl || 'notificationsJournal-itemSearch-default.html'
        },
        scope: {
            blade: '='
        },
        link: function ($scope) {
            var blade = $scope.blade;
            $scope.$localStorage = $localStorage;
            var filter = $scope.filter = blade.filter;

            if ($localStorage.notificationsJournalSearchFilterId && !filter.keyword && filter.keyword !== null) {
                filter.current = _.findWhere($localStorage.notificationsJournalSearchFilters, { id: $localStorage.notificationsJournalSearchFilterId });
                filter.keyword = filter.current ? filter.current.keyword : '';
            }
            
            function showFilterDetailBlade(bladeData) {
                var newBlade = {
                    id: 'filterDetail',
                    controller: 'virtoCommerce.notificationsModule.filterJournalDetailController',
                    template: 'Modules/$(VirtoCommerce.Notifications)/Scripts/blades/filter-detail.tpl.html',
                };
                angular.extend(newBlade, bladeData);
                bladeNavigationService.showBlade(newBlade, blade);
            };

            filter.change = function (isDetailBladeOpen) {
                $localStorage.notificationsJournalSearchFilterId = filter.current ? filter.current.id : null;
                if (filter.current && !filter.current.id) {
                    filter.current = null;
                    showFilterDetailBlade({ isNew: true });
                } else {
                    if (!isDetailBladeOpen)
                        bladeNavigationService.closeBlade({ id: 'filterJournalDetail' });
                    filter.keyword = filter.current ? filter.current.keyword : '';
                    filter.criteriaChanged();
                }
            };

            filter.edit = function ($event, entry) {
                filter.current = entry;
                showFilterDetailBlade({ data: entry });
            };
        }
    }
}]);