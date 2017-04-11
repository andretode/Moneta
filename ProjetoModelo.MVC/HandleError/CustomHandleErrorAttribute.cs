using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moneta.MVC.HandleError
{
    /// <summary>
    /// Customizei esta classe para pode fazer o ELMAH funcionar junto com a aplicação 
    /// DDD Modelo que precisa adicioanr o filter HandleErrorAttribute, e estava 
    /// incompatível, pois para o ELMAH não podia adicionar este filter.
    /// </summary>
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            if (!context.ExceptionHandled)
                return;
            var httpContext = context.HttpContext.ApplicationInstance.Context;
            var signal = ErrorSignal.FromContext(httpContext);
            signal.Raise(context.Exception, httpContext);
        }
    }
}