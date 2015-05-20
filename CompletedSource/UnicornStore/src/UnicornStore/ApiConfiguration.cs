using Microsoft.AspNet.Mvc;
using Microsoft.Framework.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace UnicornStore
{
    public static class ApiConfiguration
    {
        public static IServiceCollection ConfigureMvcOptions(this IServiceCollection services)
        {
            return services.Configure<MvcOptions>(ConfigureMvcOptions);
        }


        // Here the most important thing we do is handle circular references.
        // Todo: make this pluggable?
        public static void ConfigureMvcOptions(this MvcOptions mvcOptions)
        {
            if (mvcOptions == null) { return; } // should only occur during testing

            var jsonOutputFormatter = new JsonOutputFormatter();

            // Pro: nice default for many SPA clients. Con: Don't do this to Breeze client which has its process (NamingConvention)
            //jsonOutputFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Pro: removes most null properties reducing payload. Con: prevents update of cached entity property to a null value
            //jsonOutputFormatter.SerializerSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore; 

            // Handle circular references, e.g., Product-> Category -> Products
            jsonOutputFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;

            // Breeze client wants the typename ($type)
            jsonOutputFormatter.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects;
            mvcOptions.OutputFormatters.RemoveTypesOf<JsonOutputFormatter>();
            mvcOptions.OutputFormatters.Insert(0, jsonOutputFormatter);
        }
    }
}
