using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Castle.MicroKernel;
using Castle.Windsor;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using Owin;
using WebClient;
using WebClient.SignalR;

[assembly:OwinStartup(typeof(SignarRStartup))]
namespace WebClient
{
    public class SignarRStartup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = MvcApplication.WindsorContainer;
            container.Install(new HubsInstaller());
            GlobalHost.DependencyResolver.Register(typeof(IHubActivator), () => new CustomHubActivator(container));
            app.MapSignalR(new HubConfiguration());
        }
    }

    internal class CustomHubActivator : IHubActivator
    {
        private IWindsorContainer _container;

        internal CustomHubActivator(IWindsorContainer container) : base()
        {
            _container = container;
        }

        public IHub Create(HubDescriptor descriptor)
        {
            return _container.Resolve(descriptor.HubType) as IHub;
        }
    }
}