(function() {
    'use strict';
    angular.module('app')
        .controller('manageProductsController', manageProductsController);

    manageProductsController.$inject = ['$scope', '$timeout', 'dataservice', 'logger', 'wip-service'];
    function manageProductsController($scope, $timeout, dataservice, logger, wip) {
        var vm = this;
        vm.categories = [];
        vm.isBusy = true;
        vm.product;
        vm.products = [];
        vm.wipMessages = [];

        var wipMsgCount = 0;
        reportWipMessages();
        dataservice.ready().then(onReady).catch(handleError);
        ////////////////////
        function onReady(){
            // members that depend on a ready dataservice
            vm.addProduct = addProduct;
            vm.categories = dataservice.categories;
            vm.deleteProduct = deleteProduct;
            vm.refresh = refresh;
            vm.revertProduct = revertProduct;
            vm.saveProduct = saveProduct;
            vm.selectProduct = selectProduct;

            // add calculated properties to vm
            Object.defineProperty(vm, 'productHasChanges', {
                get: function () {
                    return vm.product && !vm.product.entityAspect.entityState.isUnchanged();
                }
            });
            Object.defineProperty(vm, 'productState', {
                get: function () {
                    return vm.product && vm.product.entityAspect.entityState.name;
                }
            })
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
            var err = typeof error === 'string'? error : (error.message || 'unknown error');
            logger.error(err);
        }

        function loadProducts(){
            vm.isBusy = true;
            vm.product = null;
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
            vm.product = null;
            return dataservice.reset().then(querySuccess, handleError);
        }
      
        function revertProduct() {
            var product = dataservice.revertEntity(vm.product);
            cleanupIfProductDetached(product);
        }

        function cleanupIfProductDetached(product) {
            if (product && product.entityAspect.entityState.isDetached()) {
                if (vm.product === product) { vm.product = null; }
                var ix = vm.products.indexOf(product);
                vm.products.splice(ix, ix < 0 ? 0 : 1);
            }
        }

        function saveProduct() {
            dataservice.saveEntity(vm.product)
                .then(function (product) {
                    cleanupIfProductDetached(product);
                });
        }

        function selectProduct(product) {
            if (product && (product.isPartial || product != vm.product)) {
                dataservice.getProductById(product.productId)
                .then(function (product) {
                    vm.product = product;
                })
            }
        }

        function sync(){
            addProduct(); // might have one pending
            vm.isBusy = true;
            return dataservice.sync().then(querySuccess, handleError);
        }

    }
})();