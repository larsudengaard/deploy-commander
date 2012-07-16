using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Deploy.King.Builds;
using Deploy.King.Executor;
using Deploy.King.Infrastructure.Factories;
using Castle.Facilities.TypedFactory;
using Deploy.Procedures;
using Deploy.Procedures.Builds;

namespace Deploy.King.Infrastructure.Installers
{
    public class ProceduresInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(AllTypes.FromAssemblyContaining(typeof (IProcedure))
                                   .BasedOn(typeof (IProcedure))
                                   .WithService.AllInterfaces()
                                   .LifestyleTransient());

            container.Register(Component.For<IBuildRepository>().ImplementedBy<TeamcityBuildRepository>().LifeStyle.Transient);
            container.Register(Component.For<IProcedureFactory>().AsFactory(x => x.SelectedWith<CreateGenericVersionOfInterfaceByArgumentsTypeSelector>()).LifeStyle.Transient);
        }
    }
}