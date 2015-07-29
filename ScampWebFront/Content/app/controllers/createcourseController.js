'use strict';
app.controller('createcourseController', function ($scope, $location, $http, CoursesFactory) {

    $scope.create = function (name, expiration) {
        var course = {
            "name": name.trim(),
            "expirationTime": expiration
        };

        var newCourse = CoursesFactory.create(course, function () {
            $location.path("home");

            //$location.path("editcourse")
            //    .search({ id: newCourse.Id });
        });
    };
});

function dumpObj(root) {
    var output = ';'
    for (var property in root) {
        output += property + ' = ' + root[property] + "\n";
    }

    return output;
}