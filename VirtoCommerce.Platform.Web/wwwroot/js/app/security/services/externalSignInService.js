angular.module('platformWebApp')
    .factory('platformWebApp.externalSignInService', ['$http', '$q', function ($http, $q) {
        return {
            getProviders: function () {
                // TODO: uncomment this after implementing external sign-in
                //return $http.get('externalsignin/providers');

                return $q(function(resolve) {
                    resolve([]);
                });
            }
        }
    }]);
