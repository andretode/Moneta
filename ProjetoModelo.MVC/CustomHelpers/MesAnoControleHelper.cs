using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Moneta.MVC.CustomHelpers
{
    public static class MesAnoControleHelper
    {
        public static MvcHtmlString MesAnoControle(this HtmlHelper htmlHelper, DateTime mesAnoCompetencia, Guid contaIdFiltro, string actionName, string controllerName)
        {
            string formAction = "/" + controllerName + "/" + actionName;
            string hrefAnterior = "href='/" + controllerName + "/" + actionName + "?mesAnoCompetencia=" + 
                mesAnoCompetencia.ToString("yyyy-MM-dd") + "&contaIdFiltro=" + contaIdFiltro + "&addMonths=-1'";
            string strMesAno = mesAnoCompetencia.ToString("dd/MM/yy");
            string hrefProximo = "href='/" + controllerName + "/" + actionName + "?mesAnoCompetencia=" +
                mesAnoCompetencia.ToString("yyyy-MM-dd") + "&contaIdFiltro=" + contaIdFiltro + "&addMonths=+1'";

            var parametros = new string[] { formAction, hrefAnterior, strMesAno, contaIdFiltro.ToString(), 
                hrefProximo };
            
            string html = @"
                <form id='formAnoMesCompetencia' action='{0}' method='post'>
                    <div style='width:100%;background-color:#A8A8A8;display:flex;min-height:30px;align-items:center;z-index:-1'>
                        <a class='text-right col-xs-3 col-md-3' title='Mês Anterior' {1} >
                            <i class='glyphicon glyphicon-chevron-left' style='color:black'></i>
                        </a>
                        <div class='text-center col-xs-6 col-md-6'>
                            <input class='visible-xs visible-sm form-control text-box text-center' type='datetime' name='mesAnoCompetencia' id='mesAnoCompetencia' 
                                value='{2}' aria-invalid='false' style='background-color:#CDCDCD;margin:auto;font-weight:bold;' onchange='enviar()' onkeydown='isKeyEnter()'>
                            <input class='visible-md visible-lg form-control datepicker text-box text-center' type='datetime' name='mesAnoCompetencia' id='mesAnoCompetencia' 
                                value='{2}' aria-invalid='false' style='background-color:#CDCDCD;margin:auto;font-weight:bold;' onchange='enviar()' onkeydown='isKeyEnter()'>
                            <input id='contaIdFiltro' name='contaIdFiltro' type='hidden' value='{3}'>
                            <input id='addMonths' name='addMonths' type='hidden' value='0'>
                        </div>
                        <a class='text-left col-xs-3 col-md-3' title='Próximo mês' {4} >
                            <i class='glyphicon glyphicon-chevron-right' style='color:black'></i>
                        </a>
                    </div>
                </form>
                <script>
                    function enviar() {{
                        var form = document.getElementById('formAnoMesCompetencia');
                        form.submit();
                    }}
                    function isKeyEnter() {{
                        if(event.keyCode == 13) {{
                            enviar();
                        }}
                    }}
                </script>
                ";
            html = String.Format(html, parametros);
            
            return MvcHtmlString.Create(html);
        }
    }
}