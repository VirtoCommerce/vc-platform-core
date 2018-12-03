import 'uikit';
import 'marked';
import 'uikit/dist/css/components/htmleditor.css';

var moduleName = "virtoCommerce.coreModule";

if (AppDependencies != undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [
    'virtoCommerce.coreModule.packageType',
    'virtoCommerce.coreModule.currency',
    'virtoCommerce.coreModule.seo',
    'virtoCommerce.coreModule.common'
]);
