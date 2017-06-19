(function () {
    app.factory('RecipeService', ['$http', function ($http) {

        var recipesEndpoint = '/api/recipe'

        var recipeService = {

            getRecipes: function (query, page) {

                var route = recipesEndpoint + '/search';

                return $http.get(route, {
                    params: {
                        query: query,
                        page: page
                    }
                })
            }
        }

        return recipeService;
    }]);
})();