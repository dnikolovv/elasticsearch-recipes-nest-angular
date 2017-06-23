(function () {
    app.controller('DetailsController', ['recipe', function (recipe) {
        var vm = this;
        vm.recipe = recipe;
    }]);
})();