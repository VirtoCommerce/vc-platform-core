angular.module('virtoCommerce.marketingModule')
.controller('virtoCommerce.marketingModule.couponImportController', ['$scope', 'platformWebApp.bladeNavigationService', 'FileUploader', 'virtoCommerce.marketingModule.promotions', function ($scope, bladeNavigationService, FileUploader, promotionsApi) {
    var blade = $scope.blade;
    blade.headIcon = 'fa-download';
    blade.isLoading = false;

    if (!$scope.uploader) {
        var uploader = $scope.uploader = new FileUploader({
            scope: $scope,
            headers: { Accept: 'application/json' },
            url: 'api/platform/assets?folderUrl=tmp',
            method: 'POST',
            autoUpload: true,
            removeAfterUpload: true
        });
        uploader.filters.push({
            name: 'csvFilter',
            fn: function (i, options) {
                var type = '|' + i.type.slice(i.type.lastIndexOf('/') + 1) + '|';
                return '|csv|vnd.ms-excel|'.indexOf(type) !== -1;
            }
        });
        uploader.onBeforeUploadItem = function (fileItem) {
            blade.isLoading = true;
        };
        uploader.onSuccessItem = function (fileItem, asset, status, headers) {
            blade.isLoading = false;
            blade.canImport = true;
            blade.csvFileUrl = asset[0].relativeUrl;
        }
        uploader.onAfterAddingAll = function (addedItems) {
            bladeNavigationService.setError(null, blade);
        };
        uploader.onErrorItem = function (item, response, status, headers) {
            bladeNavigationService.setError(item._file.name + ' failed: ' + (response.message ? response.message : status), blade);
        };
    }

    $scope.setForm = function (form) {
        $scope.formScope = form;
    };

    $scope.columnDelimiters = [{
        name: 'marketing.blades.coupons-import.labels.space',
        value: ' '
    }, {
        name: 'marketing.blades.coupons-import.labels.comma',
        value: ','
    }, {
        name: 'marketing.blades.coupons-import.labels.semicolon',
        value: ';'
    }, {
        name: 'marketing.blades.coupons-import.labels.tab',
        value: '\t'
    }];

    $scope.datepickers = {
        expirationDate: false
    };

    $scope.open = function ($event, which) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.datepickers[which] = true;
    };

    $scope.startImport = function () {
        var request = {
            fileUrl: blade.csvFileUrl,
            delimiter: blade.columnDelimiter,
            promotionId: blade.promotionId,
            expirationDate: blade.expirationDate
        };
        promotionsApi.importCoupons(request, function (response) {
            var newBlade = {
                id: 'importProgress',
                title: 'marketing.blades.coupons-import-processing.title',
                promotionId: blade.promotionId,
                notification: response,
                controller: 'virtoCommerce.marketingModule.couponImportProcessingController',
                template: 'Modules/$(VirtoCommerce.Marketing)/Scripts/promotion/blades/coupon-import-processing.tpl.html'
            };
            $scope.$on("new-notification-event", function (event, notification) {
                if (notification && notification.id == newBlade.notification.id) {
                    blade.canImport = notification.finished != null;
                }
            });
            blade.canImport = false;
            bladeNavigationService.showBlade(newBlade, blade);
        }, function (error) {
            bladeNavigationService.setError('Error ' + error.status, blade);
        });
    }
}]);