using Moneta.MVC.HandleError;
using System.Web.Mvc;

namespace Moneta.MVC
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomHandleErrorAttribute());
        }
    }
}
