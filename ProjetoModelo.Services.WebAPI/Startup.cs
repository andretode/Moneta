
using Microsoft.Owin;
using Owin;

namespace Moneta.Services.WebAPI
{
    [assembly: OwinStartup(typeof(Startup))]
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
         
        }
    }
}