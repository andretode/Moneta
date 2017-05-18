using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Moneta.MVC.CustomHelpers
{
    public static class MesAnoControleHelper
    {
        public static MvcHtmlString MesAnoControle(this HtmlHelper htmlHelper, DateTime mesAnoCompetencia, Guid contaIdFiltro, string actionName, string controllerName)
        {
            string hrefAnterior = "href='/" + controllerName + "/" + actionName + "?mesAnoCompetencia=" + 
                mesAnoCompetencia.ToString("yyyy-MM-dd") + "&contaIdFiltro=" + contaIdFiltro + "&addMonths=-1'";
            string hrefProximo = "href='/" + controllerName + "/" + actionName + "?mesAnoCompetencia=" +
                mesAnoCompetencia.ToString("yyyy-MM-dd") + "&contaIdFiltro=" + contaIdFiltro + "&addMonths=+1'";
            string hrefMesSelecionado = "/" + controllerName + "/" + actionName + "?contaIdFiltro=" + contaIdFiltro + "&addMonths=0&mesAnoCompetencia=";
            //string strMesAno = mesAnoCompetencia.ToString("MMMM yyyy").ToUpper();
            string strMesAno = mesAnoCompetencia.ToString("dd/MM/yy");

            //<input class="form-control datepicker text-box single-line hasDatepicker valid" id="DataVencimento" name="DataVencimento" type="datetime" value="05/05/17" aria-invalid="false">

            string html = @"
                <div style='width:100%;background-color:gray;color:white;display:flex;min-height: 30px;align-items:center;z-index:-1'>
                    <a class='text-right col-xs-4 col-md-4' title='Mês Anterior' {0} >
                        <i class='glyphicon glyphicon-chevron-left' style='color: white'></i>
                    </a>
                    <div class='text-center col-xs-4 col-md-4'>
                        <input class='form-control datepicker text-box text-center' type='datetime' name='DataVencimento'
                            id='DataVencimento' value='{1}' aria-invalid='false' onchange='mudarMes(""{3}"")'>
                    </div>
                    <a class='text-left col-xs-4 col-md-4' title='Próximo mês' {2} >
                        <i class='glyphicon glyphicon-chevron-right' style='color: white'></i>
                    </a>
                </div>
                ";
            html = String.Format(html, hrefAnterior, strMesAno, hrefProximo, hrefMesSelecionado);

            string jquery = @"
                <script>
                    function mudarMes(hrefMesSelecionado) {
                        var mesSelecionado = document.getElementById('DataVencimento').value;
                        var from = mesSelecionado.split('/');
                        var dataAmericana = from[2] + '-' + from[1] + '-' + from[0];
                        //httpGet(hrefMesSelecionado + dataAmericana);
                        alterarMes(hrefMesSelecionado + dataAmericana);
                    }

    function alterarMes(url) {
        alert(url);
        $.get(url, null,
            function (data) {
            }
        );
    }

                    function httpGet(theUrl)
                    {
alert(theUrl);
                        var xmlHttp = new XMLHttpRequest();
                        xmlHttp.open('GET', theUrl, true); // false for synchronous request
                        xmlHttp.send();
                    }
                </script>
            ";

            return MvcHtmlString.Create(html+jquery);
        }
    }
}