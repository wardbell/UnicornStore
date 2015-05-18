﻿using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Diagnostics.Entity;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Newtonsoft.Json.Serialization;
using UnicornStore.AspNet.Models.Identity;
using UnicornStore.AspNet.Models.UnicornStore;
using UnicornStore.Logging;
using UnicornStore.Models;

namespace UnicornStore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Setup configuration sources.
            var configuration = new Configuration()
                .AddJsonFile("config.json")
                .AddJsonFile("secrets.json") // DOESN'T EXIST IN REPO
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This reads the configuration keys from the secret store.
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                configuration.AddUserSecrets();
            }

            configuration.AddEnvironmentVariables();
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; set; }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Application settings to the services container.
            services.Configure<AppSettings>(Configuration.GetSubKey("AppSettings"));


            // Add EF services to the services container.
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<UnicornStoreContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:UnicornStore"]))
                .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:UnicornStore"]));

            services.AddSingleton<CategoryCache>();

            // Add Identity services to the services container.
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureFacebookAuthentication(options =>
            {
                options.AppId = "1593240960890768";
                options.AppSecret = Configuration.Get("secrets:facebook:appSecret");
            });

            services.ConfigureGoogleAuthentication(options =>
            {
                options.ClientId = "140672572048-92ggg4tb5ihr7ffats86pk4cgecg0cn4.apps.googleusercontent.com";
                options.ClientSecret = Configuration.Get("secrets:google:clientSecret");
            });

            // Add and configure MVC services to the services container.
            services.AddMvc();

            #region Global JSON Serialization Configuration
            // Configure JSON serialization for entire app
            // Assumes all api controller consumers want the same serialization
            // Todo: figure out how to do per-request configuration.
            services.ConfigureMvcOptions();
            #endregion

            // Uncomment the following line to add Web API services which makes it easier to port Web API 2 controllers.
            // You will also need to add the Microsoft.AspNet.Mvc.WebApiCompatShim package to the 'dependencies' section of project.json.
            // services.AddWebApiConventions();

            #region WB: Dependency Inject Demo

            services.AddSingleton<IFoo, Foo>();          // Singleton associating interface w/ a injectable, single-ctor type
            //services.AddSingleton<IFoo>(Foo.FooFactory); // Singleton w/ service locator factory

            //services.AddInstance<IFoo>(new Foo());       // Singleton with immediately-defined instance
            //services.AddInstance<IFoo>(new Foo());       // Singleton with immediately-defined instance that replaces the first one

            //services.AddScoped<IFoo>(Foo.FooFactory);    // Scoped (per request) w/ factory

            // DI creates UnicornStoreContext with its injecteds
            services.AddScoped<IUnicornStoreContext, UnicornStoreContext>();

            #endregion  
        }



        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory, IFoo foo)
        {
            // Configure the HTTP request pipeline.

            // Add the console logger.
            loggerfactory.AddConsole(minLevel: LogLevel.Warning);

            loggerfactory.AddProvider(new SqlLoggerProvider());

            // Add the following to the request pipeline only in development environment.
            if (env.IsEnvironment("Development"))
            {
                app.UseBrowserLink();
                app.UseErrorPage(ErrorPageOptions.ShowAll);
                app.UseDatabaseErrorPage(DatabaseErrorPageOptions.ShowAll);
                app.EnsureMigrationsApplied();
                app.EnsureSampleData();
            }
            else
            {
                // Add Error handling middleware which catches all application specific errors and
                // sends the request to the following path or controller action.
                app.UseErrorHandler("/Home/Error");
            }

            // Add static files to the request pipeline.
            app.UseStaticFiles();

            // Add cookie-based authentication to the request pipeline.
            app.UseIdentity();
            app.UseFacebookAuthentication();
            app.UseGoogleAuthentication();
            app.EnsureRolesCreated();
            app.ProcessPreApprovedAdmin(Configuration.Get("secrets:preApprovedAdmin"));

            // place here to capture elapsed time of MVC pages/apis but not static files
            app.UseMyMiddleware();
            //app.UseMiddleware<MyMiddlewareClass>(); // a different approach

            // Add MVC to the request pipeline.
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });

                // Uncomment the following line to add a route for porting Web API 2 controllers.
                // routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");
            });


        }

    }

}
