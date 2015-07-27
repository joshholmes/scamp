'use strict';

var services = angular.module('ScampApp.services', ['ngResource']);

var baseUrl = 'http://localhost:53463/api';

services.factory('CoursesFactory', function ($resource) {
    return $resource(baseUrl + '/course', {}, {
        query: { method: 'GET', isArray: true },
        create: { method: 'POST' }
    })
});

services.factory('CourseFactory', function ($resource) {
    return $resource(baseUrl + '/course/:id', {}, {
        show: { method: 'GET' },
        update: { method: 'PUT', params: { id: '@id' } },
        delete: { method: 'DELETE', params: { id: '@id' } },
        provision: { method: 'POST', params: { id: '@id' }, url: baseUrl + '/course/:id/resources/provision' }
    })
});

