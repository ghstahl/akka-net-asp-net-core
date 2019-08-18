using Bookstore.Contracts;
using Bookstore.Stores;
using SimpleInjector;

namespace Bookstore.Extensions
{
    public static class SimpleInjectorExtensions
    {
        public static void AddInMemoryBookstoreStore(this Container container)
        {
            container.Register<IBookstoreStore, InMemoryBookstoreStore>();
        }
    }
}
