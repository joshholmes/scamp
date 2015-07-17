'use strict';
app.controller('createcourseController', function ($scope, $location, $http, CourseFactory, CoursesFactory) {
    Init($scope, $http, CoursesFactory);

    $scope.create = function (name) {
        var course = { "name": name.trim() };

        var result = CoursesFactory.create(course);
        $location.path("home");
    };

    function Init($scope, $http, CoursesFactory) {
        $scope.courses = CoursesFactory.query();
    }
});