using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using UnicornStore.AspNet.Models.UnicornStore;

namespace UnicornStore.AspNet.Controllers
{

    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private IUnicornStoreContext db;
        private IFoo Foo {get;} 

        public ProductsController(IUnicornStoreContext dbContext, IFoo foo = null)
        {
            db = dbContext;
            Foo = foo;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var optionsAccessor = Context.RequestServices.GetRequiredService<IOptions<MvcOptions>>();
            var opts = optionsAccessor.Options;
            ConfigureOpts(opts);

            base.OnActionExecuting(context);
        }

        #region re-configure options for requests to this controller
        // Per controller re-configuration of the MvcOptions
        //
        // Essential for the `ProductsByCategory` method
        //
        // Here the most important thing we do is handle circular references.
        // Not doing this globally (in Startup.cs) because we can't be sure every controller
        // configures serialization the same way.
        private void ConfigureOpts(MvcOptions options)
        {
            //if (options == null) { throw new NullReferenceException(nameof(MvcOptions)); }
            //var jsonOutputFormatter = new JsonOutputFormatter();
            //jsonOutputFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //jsonOutputFormatter.SerializerSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore;
            //jsonOutputFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            //jsonOutputFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; // Employee.Manager

            //options.OutputFormatters.RemoveTypesOf<JsonOutputFormatter>();
            //options.OutputFormatters.Insert(0, jsonOutputFormatter);
        }
        #endregion

        //[HttpGet]
        [Route("")]
        // IQueryable doesn't really work. 
        // This filters nothing: http://localhost:5000/api/products?$filter=ProductId%20eq%202
        public IQueryable<Product> Products() 
        {
            return db.Products;
        }
        [Route("First3Products")]
        public IEnumerable<Product> First3Products()
        {
            return db.Products.Take(3);
        }

        [Route("ByCategory/{id}")]
        public IEnumerable<Product> ProductsByCategory(int id)
        {
            return db.Products
                .Where(p => p.CategoryId == id)
                .Include(p => p.Category);
        }
    }
}
