(function () {
    app.controller('SearchController', ['$state', '$stateParams', 'searchResult', function ($state, $stateParams, searchResult) {

        var vm = this;
        vm.searchResult = searchResult;

        vm.switchPage = function () {

            var params = {
                query: $stateParams.query,
                page: vm.searchResult.page,
                pageSize: $stateParams.pageSize
            };

            $state.go('recipes.search', params);
        };
    }])
})();