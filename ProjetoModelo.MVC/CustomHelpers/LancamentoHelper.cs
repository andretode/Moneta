using Moneta.Application.ViewModels;
using Moneta.Infra.CrossCutting.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            var lancamentosAgrupados = htmlHelper.ViewData.Model.LancamentosAgrupados.ToArray();
            for(int i = 0; i < lancamentosAgrupados.Count(); i++)
            {
                if (lancamentosAgrupados[i].Lancamentos.First().GrupoLancamentoId != null)
                    resultado += DisplayLancamentoAgrupado(htmlHelper, lancamentosAgrupados, i, contaIdFiltro);
                else
                    resultado += DisplayLancamentoUnico(htmlHelper, lancamentosAgrupados, i, contaIdFiltro);
            }

            return MvcHtmlString.Create(resultado.ToString());
        }

        private static string DisplayLancamentoAgrupado<TModel>(HtmlHelper<TModel> htmlHelper, IList<LancamentoAgrupadoViewModel> lancamentosAgrupados, int i, Guid? contaIdFiltro)
            where TModel : LancamentosDoMesViewModel
        {
            var strLancamento = LancamentoToJson(lancamentosAgrupados[i].Lancamentos.First());
            string html = "";

            //coluna 0 checkbox remover
            // APESAR DO LANÇAMENTO AGRUPADO ESTAR SENDO COLOCADO ESCONDIDO DEVIDO A NÃO PRECISAR DO CHECKBOX,
            // ELE AINDA PRECISA ESTAR AQUI PARA NÃO "QUEBRAR" A SEQUENCIA DE ARRAIOS,
            // O QUE PROVOCA PASSAR APENAS PARTE DA LISTA DE LANÇAMENTOS PARA A CONTROLER.
            html += "<td>";
            html += htmlHelper.CheckBoxFor(modelItem => lancamentosAgrupados[i].Lancamentos[0].Selecionado, new { @class = "hidden" } );
            html += "</td>";

            //coluna 1  TEM QUE TROCAR
            html += "<td>" + htmlHelper.DisplayFor(modelItem => lancamentosAgrupados[i].Lancamentos[0].GrupoLancamento.DataVencimento) + "&nbsp;";
            html += "<span class='visible-xs visible-sm visible-md-inline visible-lg-inline'>";
            html += "<a href='/GrupoLancamentos/Details/" + lancamentosAgrupados[i].Lancamentos[0].GrupoLancamentoId + "'>";
            html += lancamentosAgrupados[i].Descricao + "</a>";
            html += "</span>";
            html += "</td>";

            //coluna 2
            html += "<td>";
            if (lancamentosAgrupados[i].GrupoLancamento.ExtratoBancarioId != null)
            {
                html += "<a title='Clique para desfazer a conciliação' href='/Lancamentos/Desconciliar?jsonLancamento=" + strLancamento + "'>";
                html += "<span><i class='icon-white glyphicon glyphicon-link'></i>&nbsp;</span>";
                html += "</a>";
            }
            html += "<span class='visible-lg-inline'>";
            html += "<i class='icon-white glyphicon glyphicon-list' title='Este é um lançamento agrupado'></i>";
            html += "</span>";
            html += "</td>";

            //coluna 3
            html += "<td>";
            if (contaIdFiltro == Guid.Empty)
                html += "<span class='visible-md visible-lg'>" + lancamentosAgrupados[i].Lancamentos[0].Conta.Descricao + "</span>";
            html += "</td>";

            //coluna 4
            html += "<td>&nbsp;";
            html += "</td>";

            //coluna 5
            html += "<td class='text-right'>";
            html += htmlHelper.DisplayFor(l => lancamentosAgrupados[i].Lancamentos[0].GrupoLancamento.Valor);
            html += "</td>";

            //coluna 6 TEM QUE TROCAR
            html += "<td>";
            if (lancamentosAgrupados[i].Pago)
            {
                html += "<a title='Clique para informar que não foi pago' href='/Lancamentos/TrocarPago?jsonLancamento=" + strLancamento + "'>";
                html += "<i class='icon-white glyphicon glyphicon-thumbs-up' style='color:green'></i>";
                html += "</a>";
            }
            else
            {
                string corDataVencimentoPago = lancamentosAgrupados[i].GrupoLancamento.DataVencimento < DateTime.Now ? "red" : "gray";

                html += "<a title='Clique para informar que foi pago' href='/Lancamentos/TrocarPago?jsonLancamento=" + strLancamento + "'>";
                html += "<i class='icon-white glyphicon glyphicon-thumbs-down' style='color:" + corDataVencimentoPago + "'></i>";
                html += "</a>";
            }
            html += "</td>";

            html = "<tr>" + html + "</tr>";
            return html;
        }

        private static string DisplayLancamentoUnico<TModel>(HtmlHelper<TModel> htmlHelper, IList<LancamentoAgrupadoViewModel> lancamentosAgrupados, int i, Guid? contaIdFiltro)
            where TModel : LancamentosDoMesViewModel
        {
            var strLancamento = LancamentoToJson(lancamentosAgrupados[i].Lancamentos[0]);
            string html = "";

            //coluna 0
            html += "<td>";
            html += htmlHelper.CheckBoxFor(modelItem => lancamentosAgrupados[i].Lancamentos[0].Selecionado, false);
            html += htmlHelper.HiddenFor(modelItem => lancamentosAgrupados[i].Lancamentos[0].LancamentoId);
            html += htmlHelper.HiddenFor(modelItem => lancamentosAgrupados[i].Lancamentos[0].LancamentoParceladoId);
            html += htmlHelper.HiddenFor(modelItem => lancamentosAgrupados[i].Lancamentos[0].IdDaParcelaNaSerie);
            html += htmlHelper.HiddenFor(modelItem => lancamentosAgrupados[i].Lancamentos[0].CategoriaId);
            html += htmlHelper.HiddenFor(modelItem => lancamentosAgrupados[i].Lancamentos[0].ContaId);
            html += htmlHelper.HiddenFor(modelItem => lancamentosAgrupados[i].Lancamentos[0].LancamentoIdTransferencia);
            html += htmlHelper.HiddenFor(modelItem => lancamentosAgrupados[i].Lancamentos[0].DataVencimento);
            html += htmlHelper.HiddenFor(modelItem => lancamentosAgrupados[i].Lancamentos[0].DataCadastro);
            html += htmlHelper.HiddenFor(modelItem => lancamentosAgrupados[i].Lancamentos[0].Descricao);
            html += htmlHelper.HiddenFor(modelItem => lancamentosAgrupados[i].Lancamentos[0].Pago);
            html += htmlHelper.HiddenFor(modelItem => lancamentosAgrupados[i].Lancamentos[0].Valor);
            html += htmlHelper.HiddenFor(modelItem => lancamentosAgrupados[i].Lancamentos[0].Fake);
            html += "</td>";

            //coluna 1  
            html += "<td>" + htmlHelper.DisplayFor(modelItem => lancamentosAgrupados[i].Lancamentos[0].DataVencimento) + "&nbsp;";
            html += "<span class='visible-xs visible-sm visible-md-inline visible-lg-inline'>";
            if (lancamentosAgrupados[i].Lancamentos[0].TipoDeTransacao == TipoTransacaoEnum.Transferencia)
                html += htmlHelper.ActionLink(lancamentosAgrupados[i].Lancamentos[0].DescricaoResumida, "Details", "Transferencias",
                    new { @id = lancamentosAgrupados[i].Lancamentos[0].LancamentoId, @title = lancamentosAgrupados[i].Lancamentos[0].DescricaoMaisNumeroParcela }, null);
            else
            {
                html += "<a href='/Lancamentos/Details?jsonLancamento=" + strLancamento + "' title = '" +
                    lancamentosAgrupados[i].Lancamentos[0].DescricaoMaisNumeroParcela + "'>"
                    + lancamentosAgrupados[i].Lancamentos[0].DescricaoResumida + "</a>";
            }
                
            html += "</span>";
            html += "</td>";
            
            //coluna 2
            html += "<td>";
            if (lancamentosAgrupados[i].Lancamentos[0].ExtratoBancarioId != null)
            {
                html += "<a title='Clique para desfazer a conciliação' href='/Lancamentos/Desconciliar?jsonLancamento=" + strLancamento + "'>";
                html += "<span><i class='icon-white glyphicon glyphicon-link'></i>&nbsp;</span>";
                html += "</a>";
            }
            //if (lancamentosAgrupados[i].Lancamentos[0].LancamentoParceladoId != null)
            //{
            //    html += "<span class='visible-xs visible-sm'><a href='#legendas'><i class='icon-white glyphicon glyphicon-retweet' title='Este lançamento se repete em outras datas'></i></a></span>";
            //    html += "<span class='visible-md-inline visible-lg-inline'><i class='icon-white glyphicon glyphicon-retweet' title='Este lançamento se repete em outras datas'></i>&nbsp;</span>";
            //}
            if (lancamentosAgrupados[i].Lancamentos[0].TipoDeTransacao == TipoTransacaoEnum.Transferencia && lancamentosAgrupados[i].Lancamentos[0].LancamentoIdTransferencia != null)
            {
                html += "<span class='visible-xs visible-sm'><a href='#legendas'><i class='icon-white glyphicon glyphicon-transfer' title='Transferência entre contas'></i></a></span>";
                html += "<span class='visible-md-inline visible-lg-inline'><i class='icon-white glyphicon glyphicon-transfer' title='Transferência entre contas'></i>&nbsp;</span>";
            }
            html += "</td>";

            //coluna 3
            html += "<td>";
            if (contaIdFiltro == Guid.Empty)
                html += "<span class='visible-md visible-lg'>" + lancamentosAgrupados[i].Lancamentos[0].Conta.Descricao + "</span>";
            html += "</td>";

            //coluna 4
            html += "<td>";
            html += "<button class='btn' style='background-color:" + lancamentosAgrupados[i].Lancamentos[0].Categoria.Cor + "' title=" + lancamentosAgrupados[i].Lancamentos[0].Categoria.Descricao + "></button>";
            html += "<span class='visible-xs visible-sm visible-md-inline visible-lg-inline'> &nbsp;" + lancamentosAgrupados[i].Lancamentos[0].Categoria.Descricao + "</span>";
            html += "</td>";

            //coluna 5
            html += "<td class='text-right'>";
            html += htmlHelper.DisplayFor(l => lancamentosAgrupados[i].Lancamentos[0].Valor);
            html += "</td>";

            //coluna 6
            html += "<td>";
            if (lancamentosAgrupados[i].Lancamentos[0].Pago)
            {
                html += "<a title='Clique para informar que não foi pago' href='/Lancamentos/TrocarPago?jsonLancamento=" + strLancamento + "'>";
                html += "<i class='icon-white glyphicon glyphicon-thumbs-up' style='color:green'></i>";
                html += "</a>";
            }
            else
            {
                string corDataVencimentoPago = "gray";
                if (!lancamentosAgrupados[i].Lancamentos[0].Pago && lancamentosAgrupados[i].Lancamentos[0].DataVencimento < DateTime.Now)
                    corDataVencimentoPago = "red";

                html += "<a title='Clique para informar que foi pago' href='/Lancamentos/TrocarPago?jsonLancamento=" + strLancamento + "'>";
                html += "<i class='icon-white glyphicon glyphicon-thumbs-down' style='color:" + corDataVencimentoPago + "'></i>";
                html += "</a>";
            }
            html += "</td>";

            html = "<tr>" + html + "</tr>";
            return html;
        }

        private static string LancamentoToJson(LancamentoViewModel lancamento)
        {
            var jsSettings = new JsonSerializerSettings
            {
                //NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            return JsonConvert.SerializeObject(lancamento, Formatting.Indented, jsSettings);
        }
    }
}