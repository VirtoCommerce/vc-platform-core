angular.module('virtoCommerce.marketingModule')
.factory('virtoCommerce.marketingModule.promotions', ['$resource', function ($resource) {
    return $resource('api/marketing/promotions/:id', null, {
        search: { url: 'api/marketing/promotions/search', method: 'POST' },
        getNew: { url: 'api/marketing/promotions/new' },
        update: { method: 'PUT' },
        searchCoupons: { url: 'api/marketing/promotions/coupons/search', method: 'POST' },
        getCoupon: { url: 'api/marketing/promotions/coupons/:id' },
        saveCoupon: { url: 'api/marketing/promotions/coupons/add', method: 'POST' },
        deleteCoupons: { url: 'api/marketing/promotions/coupons/delete', method: 'DELETE' },
        importCoupons: { url: 'api/marketing/promotions/coupons/import', method: 'POST' }
    });
}]);
