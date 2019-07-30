angular.module('virtoCommerce.exportModule')
    .controller('virtoCommerce.exportModule.exportColumnsSelectorController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
        var blade = $scope.blade;

        function initializeBlade() {
            blade.title = 'export.blades.export-settings.labels.exported-columns';
            blade.headIcon = 'fa-folder';

            var allColumns = angular.copy(blade.exportDataRequest.allColumnsOfType);
            var selectedColumns = angular.copy(blade.exportDataRequest.dataQuery.includedColumns);
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

        $scope.cancelChanges = function () {
            bladeNavigationService.closeBlade(blade);
        }

        $scope.isValid = function () {
            var allColumns = _.flatten(_.map(blade.selectedEntities, _.values));
            return allColumns.length != 0;
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
