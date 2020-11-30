using Autofac;

namespace VRT.Resume.Application.Fakes
{
    /// <summary>
    /// Singletone IoC container helper for testing purposes
    /// </summary>
    internal class FakeIocContainer
    {
        private static FakeIocContainer _instance = new FakeIocContainer();
        public static FakeIocContainer Instance => _instance;
        private FakeIocContainer()
        {
            Container = FakeDependencies.Register();
        }
        internal IContainer Container { get; }       
        
    }
}
