(function () {
    app.controller('SearchController', ['$state', '$stateParams', 'searchData', function ($state, $stateParams, searchData) {

        var vm = this;

        vm.searchData = searchData;
        console.log(searchData);

        vm.switchPage = function () {

            var params = {};
            // Horrible but works
            if (searchData.id) { // If requesting moreLikeThis
                params.id = searchData.id;
            } else {
                params.query = $stateParams.query;
                params.page = vm.searchData.page;
            }

            $state.go('recipes.search', params);
        };
    }])
})();