using System.Collections;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Deploy.Commander.Application.Assemblies;
using Deploy.Commander.Application.Builds;
using Deploy.Commander.Application.Executor;
using Deploy.Commander.Application.Messaging;
using Deploy.Procedures.Builds;
using Deploy.Procedures.Messaging;
using Deploy.Utilities;

namespace Deploy.Commander.Infrastructure.Installers
{
    public class DeployKingInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ProcedureExecutor>().LifeStyle.Transient);
            container.Register(Component.For<ProcedureLoader>().LifeStyle.Transient);
            container.Register(Component.For<IMessenger, IMessageSubscriber>().ImplementedBy<Messenger>().LifeStyle.PerWebRequest);
            container.Register(Component.For<IBuildRepository>().ImplementedBy<TeamcityBuildRepository>().LifeStyle.Transient
                                   .DependsOn(new Hashtable
                                   {
                                       {"teamcityUrl", AppSettings.GetString("TeamcityUrl")},
                                       {"packagePath", AppSettings.GetPath("PackagePath")}
                                   }));
        }
    }
}