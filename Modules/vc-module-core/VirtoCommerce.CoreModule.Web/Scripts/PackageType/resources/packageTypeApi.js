angular.module('virtoCommerce.coreModule.packageType')
.factory('virtoCommerce.coreModule.packageType.packageTypeApi', ['$resource', function ($resource) {
	return $resource('api/packageTypes', null, {
		update: { method: 'PUT' }
	});	
}]);