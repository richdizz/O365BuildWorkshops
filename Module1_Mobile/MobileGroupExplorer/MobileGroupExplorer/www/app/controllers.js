﻿(function () {
    "use strict";

    angular.module("myapp.controllers", [])

    .controller("appCtrl", ["$scope", function ($scope) {
        // Public properties that are used at the app level in the UI
        $scope.error = "";
        $scope.activeError = false;
        $scope.wait = false;

        // Public function to dismiss an active error
        $scope.dismissError = function () {
            $scope.activeError = false;
        };

        // Event to catch an error and display it in the error section
        $scope.$on("error", function (evt, val) {
            //set the error message and mark activeError to true
            $scope.error = val;
            $scope.activeError = true;

            //apply if digest not already in progress
            if (!$scope.$$phase)
                $scope.$apply();
        });

        // Event to catch and display waiting indicator
        $scope.$on("wait", function (evt, val) {
            $scope.wait = val;

            //apply if digest not already in progress
            if(!$scope.$$phase)
                $scope.$apply();
        });
    }])

    // homeCtrl provides the logic for the home screen
    .controller("homeCtrl", ["$scope", "$state", "myappService", function ($scope, $state, myappService) {
        $scope.groups = [];

        $scope.refresh = function () {
            myappService.wait(true);
            myappService.getGroups(true).then(function (data) {
                // Refresh binding
                $scope.groups = data;
                $scope.$broadcast("scroll.refreshComplete");
                myappService.wait(false);

                // Lazy load the group photos
                angular.forEach($scope.groups, function (obj, key) {
                    obj.img = "images/group.png";
                    myappService.loadPhotoAsync(obj, "groups").then(function () {
                    });
                });
            }, function (err) {
                // Stop the refresh and broadcast the error
                $scope.$broadcast("scroll.refreshComplete");
                myappService.broadcastError(err);
                myappService.wait(false);
            });
        };
        $scope.refresh();
    }])
})();