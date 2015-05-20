/*
 * dataservice encapsulates data access and model definition
 */
(function (){
    'use strict';
    angular.module('app').factory('dataservice', dataservice);

    dataservice.$inject = ['$q', 'breeze', 'entityManagerFactory', 'logger', 'wip-service'];
    function dataservice($q, breeze, entityManagerFactory, logger, wip) {

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
            getProductById:  getProductById,
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

        function addProduct(initialValues) {
            initialValues = initialValues || {};
            initialValues.isPartial = false;
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

            return EntityQuery.from('Products/Summaries').toType(productType)
                .using(manager).execute()
                .then(success).catch(queryFailed);

            function success(data){
                // Interested in what server has then we are done.
                var fetched = data.results;
                logger.info('breeze query succeeded. fetched: '+ fetched.length);

                // Blended results.
                // All local changes plus what the server query returned.
                return manager.getEntities(productType);

                // Warning: the cache will accumulate entities that
                // have been deleted by other users until it is entirely rebuilt via 'refresh'
            }
        }

        function getProductById(id, forceRemote) {
            var self = this;
            if (!forceRemote) {
                // Check cache first (synchronous)
                var entity = manager.getEntityByKey(productType, id);
                if (entity && !entity.isPartial) {
                    logger.info('Retrieved product w/ id:' + entity.id + ' from cache.', entity);
                    if (entity.entityAspect.entityState.isDeleted()) {
                        entity = null; // hide session marked-for-delete
                    }
                    // Should not need to call $apply because it is synchronous
                    return $q.when(entity);
                }
            }

            // It was not found in cache or is partial so let's query for it.
            return breeze.EntityQuery.from('products/' + id) // can't us Breeze query!
                .using(manager).execute()
                .catch(possible404)
                .then(querySucceeded)
                .catch(queryFailed);

            function querySucceeded(data) {
                entity = data.results[0]; //data.entity;
                if (!entity) {
                    logger.warning('Could not find product with id:' + id, null);
                    return null;
                }
                setIsPartial(entity, false);
                logger.info('Retrieved product with id ' + id + ' from remote data source', entity);
                //zStorage.save();
                return entity;
            }

            function possible404(error) {
                return (error.status === 404) ?
                    $q.when({ results: [] }) : // treat as if it succeeded
                    $q.reject(error); // pass error along to next
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
            logger.error(err)
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

        function setIsPartial(entity, value) {
            entity.isPartial = false;
            // not a revertable property so remove from orginalValues
            delete entity.entityAspect.originalValues['isPartial'];
        }

        function sync() {
            return manager.saveChanges()
                .then(function (){
                    logger.info('breeze save succeeded');
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
