angular.module('virtoCommerce.exportModule')
    .controller('virtoCommerce.exportModule.exportColumnsSelectorController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
        var blade = $scope.blade;

        blade.isLoading = true;

        function initializeBlade() {
            blade.title = 'export.blades.export-settings.labels.exported-columns';
            blade.headIcon = 'fa-folder';

            var allColumns = angular.copy(blade.exportDataRequest.allColumnsOfType);
            allColumns = _.sortBy(allColumns, 'group', 'name');
            var selectedColumns = angular.copy(blade.exportDataRequest.dataQuery.includedColumns);
            selectedColumns = _.sortBy(selectedColumns, 'name');
            blade.allEntities = _.groupBy(allColumns, 'group');
            blade.selectedEntities = _.groupBy(selectedColumns, 'group');
            blade.isLoading = false;
        }

        $scope.selectAllInGroup = function (groupKey) {
            blade.selectedEntities[groupKey] = blade.allEntities[groupKey];
        }

        $scope.clearAllInGroup = function (groupKey) {
            blade.selectedEntities[groupKey] = [];
        }

        $scope.sortSelected = function (groupKey) {
            blade.selectedEntities[groupKey] = _.sortBy(blade.selectedEntities[groupKey], 'name');
        }

        $scope.cancelChanges = function () {
            bladeNavigationService.closeBlade(blade);
        }

        $scope.isValid = function () {
            return _.some(blade.selectedEntities, function (item) { return item.length; });
        }

        $scope.saveChanges = function () {
            var includedColumns = _.flatten(_.map(blade.selectedEntities, _.values));
            if (blade.onSelected) {
                blade.onSelected(includedColumns);
                bladeNavigationService.closeBlade(blade);
            }
        };

        initializeBlade();

    }]);
