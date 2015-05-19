/*
 * entityManagerFactory creates the model and delivers a new EntityManager
 */
(function (){
    'use strict';
    angular.module('app').factory('entityManagerFactory', entityManagerFactory);

    entityManagerFactory.$inject = ['breeze', 'model', 'wip-service'];
    function entityManagerFactory(breeze, model, wip) {
        var manager;
        var emFactory =  {
            getEntityManager: getEntityManager
        };
        return emFactory;
        //////////////////////

        function getEntityManager(){
            if (!manager) {
                // No manager yet; create it and load its metadata
                var dataService = new breeze.DataService({
                    hasServerMetadata: false,
                    serviceName: 'api'
                });

                manager = new breeze.EntityManager({
                    dataService: dataService,
                    metadataStore: createMetadataStore()
                });
                wip.initialize(manager);
                model.setModel(manager); 
            }
            return manager;
        }

        function createMetadataStore() {
            var convention = createUnicornStoreNamingConvention();
            return new breeze.MetadataStore({namingConvention: convention });
        }

        // camelCase convention w/ special case for MSRP property
        function createUnicornStoreNamingConvention() {
            return new breeze.NamingConvention({
                serverPropertyNameToClient: function (serverPropertyName) {
                    if (serverPropertyName === 'MSRP') { return 'msrp';}
                    return serverPropertyName.substr(0, 1).toLowerCase() + serverPropertyName.substr(1);
                },
                clientPropertyNameToServer: function (clientPropertyName) {
                    if (serverPropertyName === 'msrp') { return 'MSRP'; }
                    return clientPropertyName.substr(0, 1).toUpperCase() + clientPropertyName.substr(1);
                }
            });
        }


    }
})();
