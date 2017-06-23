(function () {
    app.controller('MainController', ['$stateParams', '$state', 'RecipeService', function ($stateParams, $state, RecipeService) {
        var vm = this;
        vm.query = $stateParams.query;
        vm.pageSize = $stateParams.pageSize;

        vm.completionSuggestions = function (query) {
            if (query.length > 0) {
                return RecipeService.autocomplete(query).then(function (response) {
                    return response.data;
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

        vm.goToRecipe = function (recipe) {
            $state.go('recipes.details', { id: recipe.id });
        }
    }]);
})();