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
                templateUrl: '/Views/Home.html',
                controller: 'MainController',
                controllerAs: 'model'
            });

        $stateProvider
            .state('recipes.details', {
                url: '/:id',
                templateUrl: '/Views/Details.html',
                controller: 'DetailsController',
                controllerAs: 'model'
            });

        $stateProvider
                .state('recipes.search', {
                    url: '/search/:query?page&pageSize',
                    resolve: {
                        searchData: ['$q', 'RecipeService', '$stateParams', function ($q, RecipeService, $stateParams) {

                            if ($stateParams.query) {
                                var deferred = $q.defer();

                                RecipeService.getRecipes($stateParams.query, $stateParams.page, $stateParams.pageSize).then(function (response) {
                                    deferred.resolve(response.data);
                                });

                                return deferred.promise;
                            } else {
                                return null;
                            }
                        }]
                    },
                    templateUrl: '/Views/SearchResult.html',
                    controller: 'SearchController',
                    controllerAs: 'model'
                });

        $stateProvider
                .state('recipes.morelikethis', {
                    url: '/morelikethis/:id?page&pageSize&recipe',
                    resolve: {
                        searchData: ['$q', 'RecipeService', '$stateParams', function ($q, RecipeService, $stateParams) {

                            var deferred = $q.defer();

                            if (typeof $stateParams.page !== "Number" || $stateParams.page <= 0) {
                                $stateParams.page = 1;
                            }

                            RecipeService.moreLikeThis($stateParams.id, $stateParams.page, $stateParams.pageSize).then(function (response) {
                                deferred.resolve(response.data);
                            });

                            return deferred.promise;
                        }]
                    },
                    templateUrl: '/Views/MoreLikeThis.html',
                    controller: 'MoreLikeThisController',
                    controllerAs: 'model'
                });

        $locationProvider.html5Mode({
            enabled: true, requireBase: false
        });
    }]);
})();