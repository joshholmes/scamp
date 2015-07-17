'use strict';
app.factory('courseService', function ($http) {

    var serviceBase = '/api/course/';
    var courseFactory = {};

    var _save = function (contact) {
        //process venue to take needed properties

        var e = {
            to: contact.email,
            title: contact.emailTitle,
            text: contact.emailText
        };

        return $http.post(serviceBase, e).then(

            function (results) {
                toaster.pop('success', "Course saved Successfully", "Place saved to your bookmark!");
            },
            function (results) {
                if (results.status == 304) {
                    toaster.pop('note', "Already Bookmarked", "Already bookmarked for user: " + e.to);
                }
                else {
                    toaster.pop('error', "Faield to Bookmark", "Something went wrong while saving :-(");
                }


                return results;
            });
    };

    placesDataFactory.sendEmail = _sendEmail;

    return placesDataFactory
});