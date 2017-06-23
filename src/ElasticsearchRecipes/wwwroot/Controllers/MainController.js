(function () {
    app.controller('MainController', ['$stateParams', '$state', 'RecipeService', function ($stateParams, $state, RecipeService) {
        var vm = this;
        vm.query = $stateParams.query;

        vm.pageSize = $stateParams.pageSize;

        vm.completionSuggestions = function (query) {
            if (query.length > 0) {

                // In order to prevent uib-typeahead selecting the first item it encounters
                // we are always going to return the query typed by the user first, and then,
                // when the API responds with actual suggestions, we're just going to append them
                var suggestions = [{ name: query }];

                return RecipeService.autocomplete(query).then(function (response) {
                    // Append the suggestions to the user query
                    return suggestions.concat(response.data);
                });

                return suggestions;
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

        vm.goToSelectedItem = function (item) {
            if (item.id) {
                $state.go('recipes.details', { id: item.id });
            } else {
                // If an id is not specified, then the user has clicked his own query
                // (the first suggestion, which is automatically selected)
                vm.search();
            }
        }
    }]);
})();