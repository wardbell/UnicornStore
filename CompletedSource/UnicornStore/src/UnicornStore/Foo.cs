using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;

namespace UnicornStore
{
    public interface IFoo
    {
        string Name { get; }
    }

    public class Foo : IFoo
    {

        public static IFoo FooFactory(System.IServiceProvider serviceProvider)
        {
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            return new Foo(loggerFactory);
        }

        static int counter = 0;

        public Foo(ILoggerFactory loggerFactory = null)
        {
            if (loggerFactory != null)
            {
                var logger = loggerFactory.CreateLogger("Foo");
                logger.LogWarning("Got me a Foo provider!");
            }
            Name = "Dan Foo " + counter++;
        }

        public string Name { get; }
    }
}
