(function () {
    app.controller('SearchController', ['searchData', function (searchData) {

        console.log(searchData);
        var vm = this;
        vm.searchData = searchData;
    }])
})();