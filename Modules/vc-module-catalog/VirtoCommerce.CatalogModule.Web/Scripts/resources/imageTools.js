angular.module('virtoCommerce.catalogModule')
.factory('virtoCommerce.catalogModule.imageTools', ['$resource', function ($resource) {

    return $resource('api/image', {},
    {
        generateThumbnails: { method: 'POST', url: 'api/image/thumbnails' }
    });
}]);

