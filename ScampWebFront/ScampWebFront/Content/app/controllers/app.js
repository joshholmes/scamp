var app = angular.module('ScampApp', ['ngRoute', 'ScampApp.services', 'ngResource', 'ui.bootstrap', 'toaster']);

app.config(function ($routeProvider) {

    $routeProvider.when("/home", {
        controller: "homeController",
        templateUrl: "/content/views/home.html"
    });

    $routeProvider.when("/createcourse", {
        controller: "createcourseController",
        templateUrl: "/content/views/createcourse.html"
    });

    $routeProvider.when("/editcourse", {
        controller: "editcourseController",
        templateUrl: "/content/views/editcourse.html"
    });

    $routeProvider.otherwise({ redirectTo: "/home" });
});