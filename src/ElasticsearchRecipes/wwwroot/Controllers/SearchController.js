(function () {
    app.controller('SearchController', ['$scope', 'searchData', function ($scope, searchData) {
        $scope.var = 'Hi from angular!';
        console.log(searchData);
    }])
})();