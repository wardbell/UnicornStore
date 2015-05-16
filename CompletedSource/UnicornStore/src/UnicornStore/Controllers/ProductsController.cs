using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.DependencyInjection; // needed for other way to get MvcOptions
using Microsoft.Framework.OptionsModel;
using Newtonsoft.Json.Serialization;
using UnicornStore.AspNet.Models.UnicornStore;

namespace UnicornStore.AspNet.Controllers
{

    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private IUnicornStoreContext db;
        private IFoo Foo {get;}
        private MvcOptions MvcOptions { get; set; }

        public ProductsController(IUnicornStoreContext dbContext, IOptions<MvcOptions> mvcOptions, IFoo foo)
        {
            db = dbContext;
            Foo = foo;
            MvcOptions = mvcOptions?.Options; // could - probably should -- reconfigure it here
        }

        #region re-configure options for requests to this controller
        // Per controller re-configuration of the MvcOptions
        //
        // Essential for the `ProductsByCategory` method
        //
        // Here the most important thing we do is handle circular references.
        // Not doing this globally (in Startup.cs) because we can't be sure every controller
        // configures serialization the same way.
        private void ConfigureMvcOptions()
        {
            //if (MvcOptions == null) { return; } // should only occur during testing

            //var jsonOutputFormatter = new JsonOutputFormatter();
            //jsonOutputFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //jsonOutputFormatter.SerializerSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore;
            //jsonOutputFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            //jsonOutputFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; // Employee.Manager

            //MvcOptions.OutputFormatters.RemoveTypesOf<JsonOutputFormatter>();
            //MvcOptions.OutputFormatters.Insert(0, jsonOutputFormatter);
        }
        #endregion

        #region A way to get MvcOptions (w/o injecting in ctor)
        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    var optionsAccessor = Context.RequestServices.GetRequiredService<IOptions<MvcOptions>>();
        //    MvcOptions = optionsAccessor.Options;
        //    // could - probably should -- reconfigure it here
        //    base.OnActionExecuting(context);
        //}
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
            ConfigureMvcOptions(); // because of circular references.
            return db.Products
                .Where(p => p.CategoryId == id)
                .Include(p => p.Category);
        }
    }
}
