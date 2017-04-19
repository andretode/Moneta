using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Moneta.MVC.CustomHelpers
{
    public static class MesAnoControleHelper
    {
        public static MvcHtmlString MesAnoControle(this HtmlHelper htmlHelper, DateTime mesAnoCompetencia, Guid contaIdFiltro, string actionName, string controllerName)
        {
            string hrefAnterior = "href='/" + controllerName + "/" + actionName + "?mesAnoCompetencia=" + mesAnoCompetencia.ToString("yyyy-MM-dd") + "&contaIdFiltro=" + contaIdFiltro + "&addMonths=-1'";
            string hrefProximo = "href='/" + controllerName + "/" + actionName + "?mesAnoCompetencia=" + mesAnoCompetencia.ToString("yyyy-MM-dd") + "&contaIdFiltro=" + contaIdFiltro + "&addMonths=+1'";
            string html =
                "<div style='width:100%;background-color:gray;color:white;display:flex;min-height: 30px;align-items:center;z-index:-1'>" +
                    "<a class='text-right col-xs-4 col-md-4' title='Mês Anterior'" + hrefAnterior + " >" +
                        "<i class='glyphicon glyphicon-chevron-left' style='color: white'></i>" +
                    "</a>" +
                    "<div class='text-center col-xs-4 col-md-4'>" + mesAnoCompetencia.ToString("MMMM yyyy").ToUpper() + "</div>" +
                    "<a class='text-left col-xs-4 col-md-4' title='Próximo mês'" + hrefProximo + " >" +
                        "<i class='glyphicon glyphicon-chevron-right' style='color: white'></i>" +
                    "</a>" +
                "</div>";

            return MvcHtmlString.Create(html);
        }
    }
}