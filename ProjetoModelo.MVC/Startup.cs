using Microsoft.Owin;
using Owin;
using Moneta.MVC;

[assembly: OwinStartup(typeof(Startup))]
namespace Moneta.MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}