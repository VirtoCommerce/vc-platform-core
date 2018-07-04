angular.module('virtoCommerce.imageToolsModule')
    .factory('virtoCommerce.imageToolsModule.taskApi',
        [
            '$resource', function($resource) {
                return $resource('api/image/thumbnails/tasks/:id',
                    { id: '@id' },
                    {
                        search: { method: 'POST', url: 'api/image/thumbnails/tasks/search' },
                        update: { method: 'PUT', url: 'api/image/thumbnails/tasks' },
                        taskRun: { method: 'POST', url: 'api/image/thumbnails/tasks/run' },
                        taskCancel: { method: 'POST', url: 'api/image/thumbnails/tasks/:jobId/cancel' }
                    });
            }
        ])
    .factory('virtoCommerce.imageToolsModule.optionApi',
        [
            '$resource', function($resource) {
                return $resource('api/image/thumbnails/options/:id',
                    { id: '@id' },
                    {
                        search: { method: 'POST', url: 'api/image/thumbnails/options/search' },
                        update: { method: 'PUT', url: 'api/image/thumbnails/options' }
                    });
            }
        ]);