using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using WebClient.Controllers;
using WebClient.Models;
using WebClient.SignalR;

namespace WebClient
{
    internal class HubsInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<GameHub>().LifestyleTransient().Interceptors(typeof (GameHubValidationInterceptor)),
                Component.For<LobbyHub>().LifestyleTransient(),
                Component.For<GameHubValidationInterceptor>());
        }
    }

    internal class ModelInstallser : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<Lobby>().LifestyleSingleton());
        }
    }
}