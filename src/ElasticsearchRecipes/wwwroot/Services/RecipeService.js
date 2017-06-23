(function () {
    app.factory('RecipeService', ['$http', function ($http) {

        var recipesEndpoint = '/api/recipe/'

        var recipeService = {

            getRecipes: function (query, page, pageSize) {

                var route = recipesEndpoint + 'search';

                return $http.get(route, {
                    params: {
                        query: query,
                        page: page,
                        pageSize: pageSize
                    }
                })
            },
            getById: function (id) {
                return $http.get(recipesEndpoint, {
                    params: {
                        id: id
                    }
                });
            },
            moreLikeThis: function (id, page, pageSize) {

                var route = recipesEndpoint + 'morelikethis';

                return $http.get(route, {
                    params: {
                        id: id,
                        page: page,
                        pageSize: pageSize
                    }
                })
            },
            autocomplete: function (query) {

                if (typeof query !== "undefined" && query.length > 0) {
                    var route = recipesEndpoint + 'autocomplete';

                    return $http.get(route, {
                        params: {
                            query: query
                        }
                    })
                }
            }
        }

        return recipeService;
    }]);
})();