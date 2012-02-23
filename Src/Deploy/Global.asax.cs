using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Deploy.Infrastructure;
using Deploy.Infrastructure.Installers;
using Deploy.King.Infrastructure;
using Deploy.King.Infrastructure.Installers;

namespace Deploy
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        WindsorContainer container;

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            container = new WindsorContainer();
            container.Install(FromAssembly.InThisApplication());

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));
            DependencyResolver.SetResolver(container.Resolve, x => (object[])container.ResolveAll(x));
        }

        protected void Application_End()
        {
            container.Dispose();
        }
    }
}