using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Deploy.Utilities;
using Newtonsoft.Json;
using Raven.Client;
using Raven.Client.Document;

namespace Deploy.Commander.Infrastructure.Installers
{
    public class 
        PersistenceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IDocumentStore>().UsingFactoryMethod(DocumentStoreFactory).LifeStyle.Singleton);
        }

        static IDocumentStore DocumentStoreFactory()
        {
            var store = CreateDocumentStore();

            store.Conventions.CustomizeJsonSerializer = serializer =>
            {
                serializer.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
                serializer.TypeNameHandling = TypeNameHandling.All;
                serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            };
            store.Initialize();
            return store;
        }

        static IDocumentStore CreateDocumentStore()
        {
            return new DocumentStore
            {
                Url = AppSettings.GetString("RavenDBHost"),
            };
        }
    }
}