using System.Web;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Deploy.Pawn.Host.Infrastructure.Installers
{
    public class MvcInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(AllTypes.FromThisAssembly().BasedOn<Controller>().LifestyleTransient(),
                               AllTypes.FromThisAssembly().BasedOn<IFilterProvider>().LifestyleTransient());

            container.Register(Component.For<HttpRequestBase>().LifeStyle.PerWebRequest.UsingFactoryMethod(() => new HttpRequestWrapper(HttpContext.Current. Request)),
                               Component.For<HttpContextBase>().LifeStyle.PerWebRequest.UsingFactoryMethod(() => new HttpContextWrapper(HttpContext.Current)));
        }
    }
}