using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;

namespace WebClient
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static IWindsorContainer WindsorContainer;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Configure);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var container = new WindsorContainer();
            WindsorContainer = container;
            container.Install(new ModelInstallser());
            var windsorDependencyResolver = new WindsorDependencyResolver(container);
            ModelBinders.Binders.DefaultBinder = windsorDependencyResolver;
        }

        internal class WindsorDependencyResolver : DefaultModelBinder
        {
            private IWindsorContainer _container;

            public WindsorDependencyResolver(IWindsorContainer container)
            {
                _container = container;
            }

            protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
            {
                return _container.Resolve(modelType);
            }
        }
    }
}
