using System.Linq;
using Microsoft.Data.Entity.Query;
using Microsoft.Data.Entity.Relational.Update;
using Microsoft.Framework.Logging;

namespace UnicornStore.Logging
{
    public class SqlLoggerProvider : ILoggerProvider
    {
        private static readonly string[] _whitelist = new string[]
        {
                typeof(BatchExecutor).FullName,
                typeof(QueryContextFactory).FullName,
                //"Microsoft.Data.Entity.Relational.Update.BatchExecutor",
                //"Microsoft.Data.Entity.Query.QueryContextFactory",
        };

        public ILogger CreateLogger(string name)
        {
            if(_whitelist.Contains(name))
            {
                return new SqlLogger();
            }

            return NullLogger.Instance;
        }
    }
}
