'use strict';
app.controller('editcourseController', function ($scope, $location, $route, CourseFactory) {

    var id = $location.search().id;
    var course = CourseFactory.details({ 'id': id });
    var courseName = course.Name;

    $scope.course = course;

    $scope.getState = function () {
        switch ($scope.course.State) {
            case 0:
                return "New";
            case 1:
                return "Provisioning";
            case 2:
                return "Complete";
            case 3:
                return "Failed";
        }
    }

    $scope.addStudent = function (first, last, email) {
        var student = { "FirstName": first.trim(), "LastName": last.trim(), "MicrosoftId": email.trim() };

        CourseFactory.addStudent({ 'id': id }, student, function (result, headers) {
            $scope.course = CourseFactory.details({ 'id': id });
        });
    };

    $scope.provisionAccounts = function () {
        $scope.course.$provision({ 'id': id });

        $scope.provisionPromise = $interval(function () {
            $scope.course.$show({ 'id': id }, function () {
                // Check for all to be complete. Stop promise.
            });
        }, 3000);
    };
});