using Moneta.Application.ViewModels;
using Moneta.Infra.CrossCutting.Enums;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Moneta.MVC.CustomHelpers
{
    public static class LancamentoHelper
    {
        public static MvcHtmlString DisplayLancamento(this HtmlHelper htmlHelper, LancamentoViewModel lancamento, Guid? contaIdFiltro)
        {

            var jsSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };
            string strLancamento = JsonConvert.SerializeObject(lancamento, Formatting.Indented, jsSettings);

            string html = "";

            //coluna 1
            html += "<td>" + lancamento.DataVencimento + "&nbsp;";
            html += "<span class='visible-xs visible-sm visible-md-inline visible-lg-inline'>";
            if (lancamento.TipoDeTransacao == TipoTransacaoEnum.Transferencia)
                html += "<a href='@Html.Action('Details', 'Transferencias', new { id = lancamento.LancamentoId })'>" + lancamento.DescricaoResumida + "</a>";
            else
            {
                html += "<a href='/Lancamentos/Details?jsonLancamento=" + strLancamento + "'>";
                html += lancamento.DescricaoResumida + "</a>";
            }
                
            html += "</span>";
            html += "<input type='hidden' name='Fake' value='" + lancamento.Descricao + "' />";
            html += "</td>";
            
            //coluna 2
            html += "<td>";
            if (lancamento.LancamentoParceladoId != null)
            {
                html += "<span class='visible-lg-inline'>";
                html += "<i class='icon-white glyphicon glyphicon-retweet' title='Este é um lançamento se repete em outras datas'></i>";
                html += "</span>";
            }
            if (lancamento.TipoDeTransacao == TipoTransacaoEnum.Transferencia && lancamento.LancamentoIdTransferencia != null)
            {
                html += "<span class=visible-lg-inline>";
                html += "<i class='icon-white glyphicon glyphicon-transfer' title='Transferência entre contas'></i>";
                html += "</span>";
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
            html += "<button class='btn' style='background-color: @item.Lancamentos[0].Categoria.Cor' title=" + lancamento.Categoria.Descricao + "></button>";
            html += "<span class='visible-xs visible-sm visible-md-inline visible-lg-inline'>" + lancamento.Categoria.Descricao + "</span>";
            html += "</td>";

            //coluna 5
            html += "<td class='text-right'>";
            if (lancamento.Valor > 0)
                html += "<span style='color:green'>" + lancamento.Valor + "</span>";
            else
                html += "<span style='color:red'>" + lancamento.Valor + "</span>";
            html += "</td>";

            //coluna 6
            html += "<td>";
            if (lancamento.Pago)
            {
                html += "<a title='Clique para informar que não foi pago' href=''@Html.Action('TrocarPago', 'Lancamentos', new { jsonLancamento })>";
                html += "<i class='icon-white glyphicon glyphicon-thumbs-up' style='color:green'></i>";
                html += "</a>";
            }
            else
            {
                string corDataVencimentoPago = "gray";
                if (!lancamento.Pago && lancamento.DataVencimento < DateTime.Now)
                    corDataVencimentoPago = "red";

                html += "<a title='Clique para informar que foi pago' href='/Lancamentos/TrocarPago?jsonLancamento=" + strLancamento + "'>";
                html += "<i class='icon-white glyphicon glyphicon-thumbs-down' style='color:" + corDataVencimentoPago + "></i>";
                html += "</a>";
            }
            html += "</td>";

            return MvcHtmlString.Create(html.ToString());
        }
    }
}