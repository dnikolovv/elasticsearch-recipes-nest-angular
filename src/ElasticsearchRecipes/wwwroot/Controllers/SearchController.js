(function () {
    app.controller('SearchController', ['searchData', function (searchData) {

        console.log(searchData);
        var vm = this;

        searchData.pageSize = 10;

        vm.searchData = searchData;
    }])
})();