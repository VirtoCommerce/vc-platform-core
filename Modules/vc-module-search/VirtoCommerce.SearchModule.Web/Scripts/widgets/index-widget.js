angular.module('virtoCommerce.searchModule')
    .controller('virtoCommerce.searchModule.indexWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.searchModule.searchIndexation', 'moment', function ($scope, bladeNavigationService, searchApi, moment) {
        var blade = $scope.blade;
        var momentFormat = "YYYYMMDDHHmmss";
        $scope.loading = true;
        function refresh() {
            searchApi.getDocIndex({ documentType: $scope.widget.documentType, documentId: blade.currentEntityId }, function (data) {
                if (_.any(data)) {
                    $scope.index = data[0];
                    $scope.indexDate = moment.utc($scope.index.indexationdate, momentFormat);
                }

                $scope.loading = false;
                updateStatus();
            });
        }

        function updateStatus() {
            if (!$scope.loading && blade.currentEntity) {
                $scope.widget.UIclass = !$scope.index || ($scope.indexDate.isBefore(moment.utc(blade.currentEntity.modifiedDate, momentFormat))) ? 'error' : '';
            }
        }

        $scope.openBlade = function () {
            var newBlade = {
                id: 'detailChild',
                currentEntityId: blade.currentEntityId,
                currentEntity: blade.currentEntity,
                data: $scope.index,
                indexDate: $scope.indexDate,
                documentType: $scope.widget.documentType,
                parentRefresh: refresh,
                controller: 'virtoCommerce.searchModule.indexDetailController',
                template: 'Modules/$(VirtoCommerce.Search)/Scripts/blades/index-detail.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, blade);
        };

        $scope.$watch('blade.currentEntity', updateStatus);

        refresh();
    }]);
