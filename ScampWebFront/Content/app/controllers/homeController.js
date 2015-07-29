'use strict';

app.controller('homeController', function ($scope, CoursesFactory) {
    $scope.courses = CoursesFactory.query();
});
