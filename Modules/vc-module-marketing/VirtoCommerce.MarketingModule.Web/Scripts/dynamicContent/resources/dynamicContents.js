angular.module('virtoCommerce.marketingModule')
    .factory('virtoCommerce.marketingModule.dynamicContent.contentItems', ['$resource', function ($resource) {
        return $resource('api/marketing/contentitems/:id', null, {
            search: { method: 'POST', url: 'api/marketing/contentitems/listentries/search' },
            create: { method: 'POST' },
            update: { method: 'PUT' }
        });
    }])
    .factory('virtoCommerce.marketingModule.dynamicContent.contentPlaces', ['$resource', function ($resource) {
        return $resource('api/marketing/contentplaces/:id', null, {
            search: { method: 'POST', url: 'api/marketing/contentplaces/listentries/search' },
            update: { method: 'PUT' }
        });
    }])
    .factory('virtoCommerce.marketingModule.dynamicContent.contentPublications', ['$resource', function ($resource) {
        return $resource('api/marketing/contentpublications/:id', null, {
            search: { method: 'POST', url: 'api/marketing/contentpublications/search' },
            getNew: { url: 'api/marketing/contentpublications/new' },
            update: { method: 'PUT' }
        });
    }])
	.factory('virtoCommerce.marketingModule.dynamicContent.folders', ['$resource', function ($resource) {
	    return $resource('api/marketing/contentfolders/:id', null, {
	        get: { method: 'GET' },
	        update: { method: 'PUT' }
	    });
	}]);
