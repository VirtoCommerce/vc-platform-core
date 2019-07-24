angular.module('virtoCommerce.exportModule')
    .controller('virtoCommerce.exportModule.exportColumnsSelectorController', ['$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
        var blade = $scope.blade;

        var allColumns;

        function initializeBlade() {
            blade.title = 'export.blades.export-settings.labels.exported-columns';
            blade.headIcon = 'fa-folder';

            allColumns = _.each(angular.copy(blade.exportDataRequest.allColumnsOfType), function (column) {
                column.$selected = _.contains(_.pluck(blade.exportDataRequest.dataQuery.includedColumns, 'name'), column.name);
            });
            blade.currentEntities = _.groupBy(allColumns, 'group');
            blade.isLoading = false;
        }

        $scope.cancelChanges = function () {
            bladeNavigationService.closeBlade(blade);
        }

        $scope.isValid = function () {
            return _.any(allColumns, function (x) { return x.$selected; });
        }

        $scope.saveChanges = function () {
            var includedColumns = _.where(allColumns, { $selected: true })
            if (blade.onSelected) {
                blade.onSelected(includedColumns);
                bladeNavigationService.closeBlade(blade);
            }
        };

        initializeBlade();

    }]);
