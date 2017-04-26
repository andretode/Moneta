using Moneta.Application.ViewModels;
using Moneta.Infra.CrossCutting.Enums;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Moneta.MVC.CustomHelpers
{
    public static class LancamentoHelper
    {
        public static MvcHtmlString DisplayLancamentos<TModel>(this HtmlHelper<TModel> htmlHelper)
            where TModel : LancamentosDoMesViewModel
        {
            string resultado = "";
            var contaIdFiltro = htmlHelper.ViewData.Model.ContaIdFiltro;
            foreach (var lancamentoAgrupado in htmlHelper.ViewData.Model.LancamentosAgrupados)
            {
                if (lancamentoAgrupado.Lancamentos.First().GrupoLancamentoId != null)
                    resultado += DisplayLancamentoAgrupado(htmlHelper, lancamentoAgrupado, contaIdFiltro);
                else
                    resultado += DisplayLancamentoUnico(htmlHelper, lancamentoAgrupado.Lancamentos[0], contaIdFiltro);
            }

            return MvcHtmlString.Create(resultado.ToString());
        }

        private static string DisplayLancamentoAgrupado<TModel>(HtmlHelper<TModel> htmlHelper, LancamentoAgrupadoViewModel lancamentoAgrupado, Guid? contaIdFiltro)
            where TModel : LancamentosDoMesViewModel
        {
            string html = "";

            //coluna 1  TEM QUE TROCAR
            html += "<td>" + htmlHelper.DisplayFor(modelItem => lancamentoAgrupado.Lancamentos[0].GrupoLancamento.DataVencimento) + "&nbsp;";
            html += "<span class='visible-xs visible-sm visible-md-inline visible-lg-inline'>";
            html += lancamentoAgrupado.Descricao + "</a>";
            html += "</span>";
            html += "</td>";

            //coluna 2
            html += "<td>";
            html += "<span class='visible-lg-inline'>";
            html += "<i class='icon-white glyphicon glyphicon-list' title='Este é um lançamento agrupado'></i>";
            html += "</span>";
            html += "</td>";

            //coluna 3
            html += "<td>";
            if (contaIdFiltro == Guid.Empty)
                html += "<span class='visible-md visible-lg'>" + lancamentoAgrupado.Lancamentos[0].Conta.Descricao + "</span>";
            html += "</td>";

            //coluna 4
            html += "<td>&nbsp;";
            html += "</td>";

            //coluna 5
            html += "<td class='text-right'>";
            decimal soma = lancamentoAgrupado.Lancamentos.Sum(l => l.Valor);
            if (soma > 0)
                html += "<span style='color:green'>" + string.Format("{0:C}", soma) + "</span>";
            else
                html += "<span style='color:red'>" + string.Format("{0:C}", soma) + "</span>";
            html += "</td>";

            //coluna 6 TEM QUE TROCAR
            html += "<td>";
            if (lancamentoAgrupado.Lancamentos[0].Pago)
            {
                html += "<a title='Clique para informar que não foi pago' href='/Lancamentos/TrocarPago?id=0'>";
                html += "<i class='icon-white glyphicon glyphicon-thumbs-up' style='color:green'></i>";
                html += "</a>";
            }
            else
            {
                string corDataVencimentoPago = "gray";
                if (!lancamentoAgrupado.Lancamentos[0].Pago && lancamentoAgrupado.Lancamentos[0].DataVencimento < DateTime.Now)
                    corDataVencimentoPago = "red";

                html += "<a title='Clique para informar que foi pago' href='/Lancamentos/TrocarPago?id=0'>";
                html += "<i class='icon-white glyphicon glyphicon-thumbs-down' style='color:" + corDataVencimentoPago + "'></i>";
                html += "</a>";
            }
            html += "</td>";

            html = "<tr>" + html + "</tr>";
            return html;
        }

        private static string DisplayLancamentoUnico<TModel>(HtmlHelper<TModel> htmlHelper, LancamentoViewModel lancamento, Guid? contaIdFiltro)
            where TModel : LancamentosDoMesViewModel
        {
            var jsSettings = new JsonSerializerSettings
            {
                //NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };
            string strLancamento = JsonConvert.SerializeObject(lancamento, Formatting.Indented, jsSettings);

            string html = "";

            //coluna 1  
            html += "<td>" + htmlHelper.DisplayFor(modelItem => lancamento.DataVencimento) + "&nbsp;";
            html += "<span class='visible-xs visible-sm visible-md-inline visible-lg-inline'>";
            if (lancamento.TipoDeTransacao == TipoTransacaoEnum.Transferencia)
                html += htmlHelper.ActionLink(lancamento.DescricaoResumida, "Details", "Transferencias",
                    new { @id = lancamento.LancamentoId, @title = lancamento.DescricaoMaisNumeroParcela }, null);
            else
            {
                html += "<a href='/Lancamentos/Details?jsonLancamento=" + strLancamento + "' title = '" +
                    lancamento.DescricaoMaisNumeroParcela + "'>"
                    + lancamento.DescricaoResumida + "</a>";
            }
                
            html += "</span>";
            html += "<input type='hidden' name='Fake' value=" + lancamento.Fake + " />";
            html += "</td>";
            
            //coluna 2
            html += "<td>";
            if (lancamento.ExtratoBancarioId != null)
            {
                html += "<span class='visible-xs visible-sm'><a href='#legendas'><i class='icon-white glyphicon glyphicon-link' title='Este lançamento está conciliado'></i></a></span>";
                html += "<span class='visible-md-inline visible-lg-inline'><i class='icon-white glyphicon glyphicon-link' title='Este lançamento está conciliado'></i>&nbsp;</span>";
            }
            //if (lancamento.LancamentoParceladoId != null)
            //{
            //    html += "<span class='visible-xs visible-sm'><a href='#legendas'><i class='icon-white glyphicon glyphicon-retweet' title='Este lançamento se repete em outras datas'></i></a></span>";
            //    html += "<span class='visible-md-inline visible-lg-inline'><i class='icon-white glyphicon glyphicon-retweet' title='Este lançamento se repete em outras datas'></i>&nbsp;</span>";
            //}
            if (lancamento.TipoDeTransacao == TipoTransacaoEnum.Transferencia && lancamento.LancamentoIdTransferencia != null)
            {
                html += "<span class='visible-xs visible-sm'><a href='#legendas'><i class='icon-white glyphicon glyphicon-transfer' title='Transferência entre contas'></i></a></span>";
                html += "<span class='visible-md-inline visible-lg-inline'><i class='icon-white glyphicon glyphicon-transfer' title='Transferência entre contas'></i>&nbsp;</span>";
            }
            html += "<input type='hidden' name='Fake' value='" + lancamento.Fake + "' />";
            html += "</td>";

            //coluna 3
            html += "<td>";
            if (contaIdFiltro == Guid.Empty)
                html += "<span class='visible-md visible-lg'>" + lancamento.Conta.Descricao + "</span>";
            html += "</td>";

            //coluna 4
            html += "<td>";
            html += "<button class='btn' style='background-color:" + lancamento.Categoria.Cor + "' title=" + lancamento.Categoria.Descricao + "></button>";
            html += "<span class='visible-xs visible-sm visible-md-inline visible-lg-inline'> &nbsp;" + lancamento.Categoria.Descricao + "</span>";
            html += "</td>";

            //coluna 5
            html += "<td class='text-right'>";
            html += htmlHelper.DisplayFor(l => lancamento.Valor);
            html += "</td>";

            //coluna 6
            html += "<td>";
            if (lancamento.Pago)
            {
                html += "<a title='Clique para informar que não foi pago' href='/Lancamentos/TrocarPago?jsonLancamento=" + strLancamento + "'>";
                html += "<i class='icon-white glyphicon glyphicon-thumbs-up' style='color:green'></i>";
                html += "</a>";
            }
            else
            {
                string corDataVencimentoPago = "gray";
                if (!lancamento.Pago && lancamento.DataVencimento < DateTime.Now)
                    corDataVencimentoPago = "red";

                html += "<a title='Clique para informar que foi pago' href='/Lancamentos/TrocarPago?jsonLancamento=" + strLancamento + "'>";
                html += "<i class='icon-white glyphicon glyphicon-thumbs-down' style='color:" + corDataVencimentoPago + "'></i>";
                html += "</a>";
            }
            html += "</td>";

            html = "<tr>" + html + "</tr>";
            return html;
        }
    }
}