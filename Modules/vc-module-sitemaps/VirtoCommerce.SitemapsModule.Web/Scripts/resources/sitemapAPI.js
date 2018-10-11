angular.module('virtoCommerce.sitemapsModule')
.factory('virtoCommerce.sitemapsModule.sitemapApi', ['$resource', function ($resource) {
    return $resource('api/sitemaps/:id', {}, {
        search: { method: 'POST', url: 'api/sitemaps/search' },
        addSitemap: { method: 'POST' },
        updateSitemap: { method: 'PUT' },
        removeSitemap: { method: 'DELETE' },
        searchSitemapItems: { method: 'POST', url: 'api/sitemaps/items/search' },
        addSitemapItems: { method: 'POST', url: 'api/sitemaps/:sitemapId/items' },
        removeSitemapItems: { method: 'DELETE', url: 'api/sitemaps/:sitemapId/items' },
        download: { method: 'GET', url: 'api/sitemaps/download' }
    });
}]);