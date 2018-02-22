angular.module("virtoCommerce.coreModule.packageType", [])
.factory('virtoCommerce.coreModule.packageType.packageTypeUtils', ['virtoCommerce.coreModule.packageType.packageTypeApi', 'platformWebApp.bladeNavigationService', function (packageTypeApi, bladeNavigationService) {
	var packageTypesRef;
	return {
		getPackageTypes: function () { return packageTypesRef = packageTypeApi.query(); },
		editPackageTypes: function (blade) {
			var newBlade = {
				id: 'packageTypeList',
				parentRefresh: function (data) { angular.copy(data, packageTypesRef); },
				controller: 'virtoCommerce.coreModule.packageType.packageTypeListController',
				template: 'Modules/$(VirtoCommerce.Core)/Scripts/PackageType/blades/package-type-list.tpl.html'
			};
			bladeNavigationService.showBlade(newBlade, blade);
		}
	};
}])
;