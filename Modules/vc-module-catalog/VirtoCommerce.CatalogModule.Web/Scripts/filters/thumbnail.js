angular.module('virtoCommerce.catalogModule')
    .filter('thumbnail', function () {
        return function (imageUrl, prefix) {
            imageUrl = imageUrl.replace(/(\.[\w\d_-]+)$/i, prefix + '$1');
            return imageUrl;         
        };
    });
