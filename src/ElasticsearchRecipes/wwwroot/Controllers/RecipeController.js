(function () {
    app.controller('RecipeController', ['$stateParams', function ($stateParams) {
        var vm = this;
        vm.query = $stateParams.query;
        vm.pageSize = $stateParams.pageSize;
    }]);
})();