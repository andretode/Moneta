using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Moneta.Infra.CrossCutting.Identity.Configuration;

namespace Moneta.MVC.Controllers
{
    public class BaseController : Controller
    {
        protected Guid AppUserId
        {
            get {
                var _userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                return Guid.Parse(_userManager.FindByNameAsync(User.Identity.Name).Result.Id);
            }
        }

        protected Guid ContaId
        {
            get {
                return Guid.Parse(Util.GetCookieContaId(Request, Response).Value.ToString());
            }
        }
    }
}