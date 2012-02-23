using System.Configuration;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Newtonsoft.Json;
using Raven.Client;
using Raven.Client.Document;

namespace Deploy.King.Infrastructure.Installers
{
    public class PersistenceFacility : AbstractFacility
    {
        protected override void Init()
        {
            Kernel.Register(Component.For<IDocumentStore>().UsingFactoryMethod(DocumentStoreFactory).LifeStyle.Singleton);
            Kernel.Register(Component.For<IDocumentSession>().UsingFactoryMethod(DocumentSessionFactory).LifeStyle.Transient);
        }

        IDocumentStore DocumentStoreFactory()
        {
            var store = CreateDocumentStore();
            ConfigureStore(store);
            store.Initialize();
            return store;
        }

        protected static IDocumentSession DocumentSessionFactory(IKernel kernel)
        {
            var store = kernel.Resolve<IDocumentStore>();
            var session = store.OpenSession();
            return session;
        }

        protected virtual IDocumentStore CreateDocumentStore()
        {
            var ravenDbHost = ConfigurationManager.AppSettings["RavenDBHost"];

            if (string.IsNullOrWhiteSpace(ravenDbHost))
                throw new ConfigurationErrorsException("RavenDBHost was not set in config file");

            return new DocumentStore
            {
                Url = ravenDbHost
            };
        }

        public virtual void ConfigureStore(IDocumentStore store)
        {
            store.Conventions.CustomizeJsonSerializer = serializer =>
            {
                serializer.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
                serializer.TypeNameHandling = TypeNameHandling.All;
            };
        }
    }
}