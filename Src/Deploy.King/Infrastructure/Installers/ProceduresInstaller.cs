using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Deploy.King.Procedures;

namespace Deploy.King.Infrastructure.Installers
{
    public class ProceduresInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(AllTypes.FromAssemblyContaining(typeof (IDeployProcedure))
                                   .BasedOn(typeof (IDeployProcedure))
                                   .WithService.Base()
                                   .LifestyleTransient());
        }
    }
}