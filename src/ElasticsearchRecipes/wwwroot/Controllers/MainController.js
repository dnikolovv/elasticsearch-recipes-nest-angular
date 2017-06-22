(function () {
    app.controller('MainController', ['$stateParams', '$state', 'RecipeService', function ($stateParams, $state, RecipeService) {
        var vm = this;
        vm.query = $stateParams.query;
        vm.pageSize = $stateParams.pageSize;
        vm.completionSuggestions = [];

        vm.autocomplete = function () {
            if (vm.query.length > 0) {
                 RecipeService.autocomplete(vm.query).then(function (response) {
                    console.log('Autocomplete response: ' + response);
                    vm.completionSuggestions = response.data;
                });
            }
        }

        vm.search = function () {
            var params = {
                query: vm.query,
                pageSize: vm.pageSize,
                page: 1
            };

            $state.go('recipes.search', params);
        }
    }]);
})();