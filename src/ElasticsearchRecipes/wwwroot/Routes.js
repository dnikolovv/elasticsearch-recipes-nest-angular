(function () {
    app.config(['$locationProvider', '$stateProvider', function ($locationProvider, $stateProvider) {

        $stateProvider
            .state('default', {
                url: '/',
                redirectTo: 'recipes'
            });

        $stateProvider
            .state('recipes', {
                url: '/recipes',
                templateUrl: '/Views/Recipes.html',
                redirectTo: 'recipes.search'
            });

        $stateProvider
                .state('recipes.search', {
                    url: '/search/?query',
                    resolve: {
                        searchData: ['$q', 'RecipeService', '$stateParams', function ($q, RecipeService, $stateParams) {

                            if ($stateParams.query) {
                                var deferred = $q.defer();

                                RecipeService.getRecipes($stateParams.query).then(function (response) {
                                    deferred.resolve(response.data);
                                });

                                return deferred.promise;
                            } else {
                                return null;
                            }
                        }]
                    },
                    templateUrl: '/Views/Search.html',
                    controller: 'SearchController',
                    controllerAs: 'model'
                });

        $locationProvider.html5Mode({
            enabled: true, requireBase: false
        });
    }]);
})();