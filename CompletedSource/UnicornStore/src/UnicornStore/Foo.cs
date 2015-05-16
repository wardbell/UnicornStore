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
            if (loggerFactory != null)
            {
                var logger = loggerFactory.CreateLogger("fooFactory");
                logger.LogWarning("Got me a Foo provider!");
            }
            return new Foo();
        }

        static int counter = 0;

        public Foo()
        {
            Name = "Dan Foo " + counter++;
        }

        public string Name { get; }
    }
}
