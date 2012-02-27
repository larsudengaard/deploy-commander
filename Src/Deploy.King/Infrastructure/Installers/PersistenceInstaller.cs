using System.Configuration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Newtonsoft.Json;
using Raven.Client;
using Raven.Client.Document;

namespace Deploy.King.Infrastructure.Installers
{
    public class PersistenceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IDocumentStore>().UsingFactoryMethod(DocumentStoreFactory).LifeStyle.Singleton);
        }

        static IDocumentStore DocumentStoreFactory()
        {
            var store = CreateDocumentStore();
            
            //store.Conventions.CustomizeJsonSerializer = serializer =>
            //{
            //    serializer.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            //    serializer.TypeNameHandling = TypeNameHandling.All;
            //};
            store.Initialize();
            return store;
        }

        static IDocumentStore CreateDocumentStore()
        {
            var ravenDbHost = ConfigurationManager.AppSettings["RavenDBHost"];

            if (string.IsNullOrWhiteSpace(ravenDbHost))
                throw new ConfigurationErrorsException("RavenDBHost was not set in config file");

            return new DocumentStore
            {
                Url = ravenDbHost,
            };
        }
    }
}