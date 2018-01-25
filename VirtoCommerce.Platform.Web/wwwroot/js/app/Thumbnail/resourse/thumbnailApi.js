angular.module('platformWebApp')
    .factory('platformWebApp.thumbnail.api', ['$resource', '$timeout', function ($resource, $timeout) {
        return {
            getTaskList: function () {
                var list = [
                    {
                        id: '778810c7629c44688db7ce997c2f18f1',
                        createdDate: '2016-08-13T16:20:14.493Z',
                        modifiedDate: '2018-07-13T16:20:14.493Z',
                        createdBy: 'admin',
                        modifiedBy: 'admin',
                        name: 'Clother catalog test thumbnail',
                        lastRun: '2018-01-25T16:20:14.493Z',
                        workPath: '/catalog/clother',
                        thumbnailOptions: {
                            name: 'options',
                            fileSuffix: 'thb_',
                            resizeMethod: 'fixedWidth',
                            width: '50',
                            height: '50',
                            backgroundColor: '#555'
                        }
                    },
                    {
                        id: '397010c7629c44688db7ce997c2f18f1',
                        createdDate: '2017-07-13T16:20:14.493Z',
                        modifiedDate: '2018-07-13T16:20:14.493Z',
                        createdBy: 'admin',
                        modifiedBy: 'admin',
                        name: 'Electronics catalog test thumbnail',
                        lastRun: '2018-01-25T16:20:14.493Z',
                        workPath: '/catalog/electronics',
                        thumbnailOptions: {
                            name: 'options',
                            fileSuffix: 'thb_',
                            resizeMethod: 'crop',
                            width: '100',
                            height: '100',
                            backgroundColor: '#333333'
                        }
                    }
                ];
                return $timeout(function () { return { list}}, 1000);
            }
        }
    }]);
