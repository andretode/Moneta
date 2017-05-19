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
            string strDataDatepicker = mesAnoCompetencia.ToString("MM/dd/yyyy");
            
            string html = @"
                <form id='formAnoMesCompetencia' action='{0}' method='post'>
                    <div style='width:100%;background-color:gray;color:white;display:flex;min-height: 30px;align-items:center;z-index:-1'>
                        <a class='text-right col-xs-4 col-md-4' title='Mês Anterior' {1} >
                            <i class='glyphicon glyphicon-chevron-left' style='color: white'></i>
                        </a>
                        <div class='text-center col-xs-4 col-md-4'>
                                value='{2}' aria-invalid='false' style='background-color : gray; color:white' onchange='enviar()' onkeydown='isKeyEnter()'>
                            <input id='contaIdFiltro' name='contaIdFiltro' type='hidden' value='{3}'>
                            <input id='addMonths' name='addMonths' type='hidden' value='0'>
                        </div>
                        <a class='text-left col-xs-4 col-md-4' title='Próximo mês' {4} >
                            <i class='glyphicon glyphicon-chevron-right' style='color: white'></i>
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
            var parametros = new string[] { formAction, hrefAnterior, strMesAno, contaIdFiltro.ToString(), hrefProximo, strDataDatepicker };
            html = String.Format(html, parametros);
            
            return MvcHtmlString.Create(html);
        }
    }
}