using System.Linq;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational.Migrations;
using System;

namespace UnicornStore.AspNet.Models
{
    public static class DbContextExtensions
    {
        public static bool AllMigrationsApplied(this DbContext context)
        {
            var provider = ((IAccessor<IServiceProvider>)context).Service;
            var migrator = (Migrator)provider.GetService(typeof(IMigrator));
            return !migrator.GetUnappliedMigrations().Any();
        }
    }
}
