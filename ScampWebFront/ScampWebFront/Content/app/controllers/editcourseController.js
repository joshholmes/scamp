'use strict';
app.controller('editcourseController', function ($scope, $location, $route, CourseFactory) {
    var id = $location.search().id;
    var course = CourseFactory.show({ 'id': id });
    $scope.course = course;

    var courseName = course.Name;

    $scope.addStudent = function (first, last, email) {
        var student = { "FirstName": first.trim(), "LastName": last.trim(), "Email": email.trim() };

        if ($scope.course.Students == null) {
            $scope.course.Students = [];
        }
        $scope.course.Students.push(student);
        $scope.course.$update({ 'id': id });

        $scope.$apply();
    };
});