using Microsoft.Owin;
using Owin;
using Test2;

[assembly:OwinStartup(typeof(SignarRStartup))]
namespace Test2
{
    public class SignarRStartup
    {
        public void Configuration(IAppBuilder app)
        {
           
            app.MapSignalR();
        }
    }
}