using System;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;

namespace Deploy.Infrastructure.Installers
{
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IWindsorContainer windsorContainer;

        public WindsorControllerFactory(IWindsorContainer windsorContainer)
        {
            this.windsorContainer = windsorContainer;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return windsorContainer.Resolve(controllerType) as IController;
        }

        public override void ReleaseController(IController controller)
        {
            windsorContainer.Release(controller);
        }
    }
}