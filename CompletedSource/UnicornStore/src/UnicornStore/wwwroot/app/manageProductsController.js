(function() {
    'use strict';
    angular.module('app')
        .controller('manageProductsController', manageProductsController);

    manageProductsController.$inject = ['$scope', '$timeout', 'dataservice', 'wip-service'];
    function manageProductsController($scope, $timeout, dataservice, wip) {
        var vm = this;
        vm.categories = [];
        vm.currentProduct;
        vm.isBusy = true;
        vm.products = [];
        vm.wipMessages = [];

        var wipMsgCount = 0;
        reportWipMessages();
        dataservice.ready().then(onReady).catch(handleError);
        ////////////////////
        function onReady(){
            // members that depend on a ready dataservice
            vm.addProduct = addProduct;
            vm.counts = dataservice.counts;
            vm.deleteProduct = deleteProduct;
            vm.refresh = refresh;
            vm.reset = reset;
            vm.sync = sync;

            loadProducts(); // initial data load          
        }

        ////////////////////////////
        function addProduct() {
            //TBD
            if (vm.newProductText !== '') {
                var newProduct = dataservice.addProduct({ text: vm.newProductText});
                vm.products.unshift(newProduct);
                vm.newProductText='';
            }
        }

        function deleteProduct(product) {
            //TBD
            dataservice.deleteProduct(product);
            if (product.entityAspect.entityState.isDetached()){
                // remove from the list if became detached
                var ix = vm.products.indexOf(product);
                if (ix > -1) { vm.products.splice(ix,1); }
            }
        }

        function getProducts() {
            vm.isBusy = true;
            // Controller has no knowledge of how data
            // is retrieved nor that Breeze is involved.
            return dataservice.getProducts()
                .then(querySuccess, handleError);
        }

        function handleError(error) {
            vm.isBusy = false;  
            var err = typeof error === 'string'? error : error.message;
            vm.errorLog.push((vm.errorLog.length+1) + ': ' + 
                (err || 'unknown error'));
        }

        function loadProducts(){
            vm.isBusy = true;
            return dataservice.loadProducts()
            .then(querySuccess, handleError);
        }

        function querySuccess(products){
            vm.isBusy = false;   
            vm.products = products;
        }

        function refresh(){
            addProduct();  // might have one pending
            return getProducts();
        }

        function reportWipMessages(){
            $scope.$on(wip.eventName(), function(event, message){
                vm.wipMessages.push((wipMsgCount+=1)+' - '+message);
                $timeout(function(){vm.wipMessages.length=0;}, 8000);
            })
        }

        function reset(){
            vm.isBusy = true;
            return dataservice.reset().then(querySuccess, handleError);
        }

        function sync(){
            addProduct(); // might have one pending
            vm.isBusy = true;
            return dataservice.sync().then(querySuccess, handleError);
        }

    }
})();