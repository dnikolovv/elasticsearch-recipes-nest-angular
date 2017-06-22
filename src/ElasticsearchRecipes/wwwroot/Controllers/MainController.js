(function () {
    app.controller('MainController', ['$stateParams', function ($stateParams) {
        var vm = this;
        vm.query = $stateParams.query;
        vm.pageSize = $stateParams.pageSize;
    }]);
})();