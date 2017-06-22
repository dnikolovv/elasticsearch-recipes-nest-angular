(function () {
    app.factory('RecipeService', ['$http', function ($http) {

        var recipesEndpoint = '/api/recipe'

        var recipeService = {

            getRecipes: function (query, page, pageSize) {

                var route = recipesEndpoint + '/search';

                return $http.get(route, {
                    params: {
                        query: query,
                        page: page,
                        pageSize: pageSize
                    }
                })
            },
            moreLikeThis: function (id, page) {

                var route = recipesEndpoint + '/morelikethis';

                return $http.get(route, {
                    params: {
                        id: id,
                        page: page
                    }
                })
            }
        }

        return recipeService;
    }]);
})();