using System.Web.Http;
using Moneta.Application.AutoMapper;
using System;
using System.Linq;
using System.Web;

namespace Moneta.Services.WebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AutoMapperConfig.RegisterMappings();
        }
    }
}
