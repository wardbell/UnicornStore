/*
 * dataservice encapsulates data access and model definition
 */
(function (){
    'use strict';
    angular.module('app').factory('dataservice', dataservice);

    dataservice.$inject = ['$log', '$q', 'breeze', 'entityManagerFactory', 'wip-service'];
    function dataservice($log, $q, breeze, entityManagerFactory, wip) {

        var addedState = breeze.EntityState.Added;
        var deletedState = breeze.EntityState.Deleted;
        var EntityQuery = breeze.EntityQuery;
        var manager;
        var productType;

        var ds = {
            addProduct:      addProduct,
            categories:      [],
            counts:          {},
            deleteProduct:   deleteProduct,
            getProducts:     getProducts,
            hasChanges:      hasChanges,
            loadProducts:    loadProducts,
            ready:           ready,
            reset:           reset,
            sync:            sync
        };

        return ds;
        /////////////////////////////
        function ready(){
            manager = entityManagerFactory.getEntityManager();
            manager.entityChanged.subscribe(entityCountsChanged);
            productType = manager.metadataStore.getEntityType('Product');
            updateCounts();

            // categories (a lookup) must be loaded before anyone can use the service
            var catQuery = EntityQuery.from('Categories').using(manager);
            
            ds.categories = catQuery.executeLocally();
            if (ds.categories.length === 0) {
                return catQuery.execute()
                    .then(function (data) {
                        ds.categories = data.results;
                    })
            } else {
                return $q.when(true);
            }
        }

        function addProduct(initialValues){
            return manager.createEntity(productType, initialValues);
        }

        function deleteProduct(product){
            var aspect = product.entityAspect;
            if (aspect.entityState !== breeze.EntityState.Detached){
                aspect.setDeleted();
            }
        }

        function entityCountsChanged(changeArgs){
            var action = changeArgs.entityAction;
            if (action !== breeze.EntityAction.PropertyChange){
                updateCounts();
            }
        }





        // Get Products from the server (just summaries) and cache combined
        function getProducts() {

            EntityQuery.from('Products/Summaries').toType(productType)
            .using(manager).execute()
            .then(success).catch(queryFailed);

            function success(data){
                // Interested in what server has then we are done.
                var fetched = data.results;
                $log.log('breeze query succeeded. fetched: '+ fetched.length);

                // Blended results.
                // All local changes plus what the server query returned.
                return manager.getEntities(productType);

                // Warning: the cache will accumulate entities that
                // have been deleted by other users until it is entirely rebuilt via 'refresh'
            }
        }







        function hasChanges(){
            return manager.hasChanges();
        }

        function loadProducts(){
            wip.restore();
            return getProducts();
        }

        function queryFailed(error) {
            var status = error.status ? error.status + ' - ' : '';
            var err = status + (error.message ? error.message : 'Unknown error; check console.log.');
            err += '\nIs the server running?';
            return $q.reject(err); // so downstream listener gets it.
        }

        // Clear everything local and reload from server.
        function reset(){
            wip.stop();
            wip.clear();
            manager.clear();
            return getAllProducts()
                .finally(function(){wip.resume();});
        }

        function sync() {
            return manager.saveChanges()
                .then(function (){
                    $log.log('breeze save succeeded');
                    wip.clear();
                    return getAllProducts();
                })
                .catch(saveFailed);

            function saveFailed(error) {
                var msg = 'Save failed: ' +
                    breeze.saveErrorMessageService.getErrorMessage(error);
                error.message = msg;
                throw error; // for downstream callers to see
            }
        }

        function updateCounts() {
            var counts = ds.counts;
            counts.all = 0;
            counts.Added = 0;
            counts.Deleted = 0;
            counts.Modified = 0;
            counts.Unchanged = 0;
            manager.getEntities().forEach(countIt);

            function countIt(entity){
                var state = entity.entityAspect.entityState.name;
                counts[state] += 1;
                counts.all += 1;
            }
        }
    }
})();
