﻿using System;
using System.Linq;
using System.Web.Mvc;
using Moneta.Application.Interfaces;
using Moneta.Application.ViewModels;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using Moneta.Infra.CrossCutting.Enums;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Moneta.MVC.Controllers
{
    [Authorize(Roles = "UsuarioPlanoBasico")]
    public class ExtratosBancariosController : Controller
    {
        private readonly IExtratoBancarioAppService _ExtratoBancarioApp;
        private readonly ILancamentoAppService _LancamentoAppService;
        private readonly IGrupoLancamentoAppService _GrupoLancamentoAppService;
        private readonly IContaAppService _ContaApp;

        public ExtratosBancariosController(
            IExtratoBancarioAppService extratoBancarioApp,
            ILancamentoAppService lancamentoAppService,
            IGrupoLancamentoAppService GrupoLancamentoAppService,
            IContaAppService contaApp)
        {
            _ExtratoBancarioApp = extratoBancarioApp;
            _LancamentoAppService = lancamentoAppService;
            _GrupoLancamentoAppService = GrupoLancamentoAppService;
            _ContaApp = contaApp;
        }

        public ViewResult Index(ExtratoBancarioDoMesViewModel extratosDoMes, string pesquisa, int quant = -1)
        {
            var cookieContaId = Util.GetCookieContaId(Request, Response);
            extratosDoMes.ContaIdFiltro = Guid.Parse(cookieContaId.Value.ToString());

            extratosDoMes.ExtratosDoMes = _ExtratoBancarioApp.GetExtratosDoMes(extratosDoMes.MesAnoCompetencia, (Guid)extratosDoMes.ContaIdFiltro);
            ViewBag.Pesquisa = pesquisa;
            ViewBag.quantidadeImportada = quant;
            SetSelectLists();

            return View(extratosDoMes);
        }

        [HttpPost]
        public ViewResult Index(ExtratoBancarioDoMesViewModel extratosDoMes)
        {
            var cookieContaId = Util.GetCookieContaId(Request, Response);
            extratosDoMes.ContaIdFiltro = Guid.Parse(cookieContaId.Value.ToString());

            extratosDoMes.ExtratosDoMes = _ExtratoBancarioApp.GetExtratosDoMes(extratosDoMes.MesAnoCompetencia, (Guid)extratosDoMes.contaIdFiltro);
            ViewBag.quantidadeImportada = -1;
            SetSelectLists();

            return View(extratosDoMes);
        }

        public ViewResult AlterarMes(DateTime mesAnoCompetencia, int addMonths)
        {
            mesAnoCompetencia = mesAnoCompetencia.AddMonths(addMonths);

            var extratosDoMes = new ExtratoBancarioDoMesViewModel();
            extratosDoMes.MesAnoCompetencia = mesAnoCompetencia;
            var cookieContaId = Util.GetCookieContaId(Request, Response);
            extratosDoMes.ContaIdFiltro = Guid.Parse(cookieContaId.Value.ToString());
            extratosDoMes.ExtratosDoMes = _ExtratoBancarioApp.GetExtratosDoMes(mesAnoCompetencia, (Guid)extratosDoMes.ContaIdFiltro);

            ViewBag.quantidadeImportada = -1;
            SetSelectLists();
            return View("Index", extratosDoMes);
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult CriarLancamento(Guid id)
        {
            var eb = _ExtratoBancarioApp.GetById(id);
            var tipoDeTransacao = (eb.Valor <= 0 ? TipoTransacaoEnum.Despesa : TipoTransacaoEnum.Receita);
            var lancamento = ClonarLancamento(eb, tipoDeTransacao);

            return RedirectToAction("CreateFromExtrato", "Lancamentos", lancamento);
        }

        public ActionResult CriarTransferencia(Guid id)
        {
            var eb = _ExtratoBancarioApp.GetById(id);
            var lancamento = ClonarLancamento(eb, TipoTransacaoEnum.Transferencia);

            return RedirectToAction("CreateFromExtrato", "Transferencias", lancamento);
        }

        private LancamentoViewModel ClonarLancamento(ExtratoBancarioViewModel extrato, TipoTransacaoEnum tipoTransacao)
        {
            var eb = extrato;
            var lancamento = new LancamentoViewModel()
            {
                Conta = eb.Conta,
                ContaId = eb.ContaId,
                DataVencimento = eb.DataCompensacao,
                Descricao = eb.Descricao,
                ExtratoBancarioId = eb.ExtratoBancarioId,
                Fake = false,
                Ativo = true,
                NumeroDocumento = eb.NumeroDocumento,
                Pago = true,
                Valor = (tipoTransacao == TipoTransacaoEnum.Transferencia ? eb.Valor : Math.Abs(eb.Valor)),
                TipoDeTransacao = tipoTransacao
            };

            return lancamento;
        }

        public ActionResult ImportarOfx(ExtratoBancarioDoMesViewModel extratoDoMes)
        {
            SetSelectLists();
            return View(extratoDoMes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportarOfx(string arquivoOfx, DateTime MesAnoCompetencia)
        {
            int quantidadeImportada = 0;
            string caminhoOfx = "";
            var cookieContaId = Util.GetCookieContaId(Request, Response);
            var ContaIdFiltro = Guid.Parse(cookieContaId.Value.ToString());
            var extratoDoMes = new ExtratoBancarioDoMesViewModel() { ContaIdFiltro = ContaIdFiltro, MesAnoCompetencia = MesAnoCompetencia };

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    caminhoOfx = Path.Combine(Server.MapPath("~/"), fileName);
                    file.SaveAs(caminhoOfx);
                }
            }

            if (caminhoOfx == "")
            {
                ModelState.AddModelError(string.Empty, "Ocorreu um erro ao salvar o arquivo OFX enviado.");
                SetSelectLists();
                return View(extratoDoMes);
            }

            try
            {
                quantidadeImportada = _ExtratoBancarioApp.ImportarOfx(caminhoOfx, ContaIdFiltro, MesAnoCompetencia);
            }
            catch (FormatException fx)
            {
                ModelState.AddModelError(string.Empty, "O arquivo enviado tem um formato OFX desconhecido. Detalhes técnicos do erro: " + fx.Message);
                SetSelectLists();
                return View(extratoDoMes);
            }
            catch (XmlException xe)
            {
                ModelState.AddModelError(string.Empty, "O arquivo enviado tem um formato OFX fora do padrão. Detalhes técnicos do erro: " + xe.Message);
                SetSelectLists();
                return View(extratoDoMes);
            }
            
            return RedirectToAction("Index", new { quant = quantidadeImportada, ContaIdFiltro, MesAnoCompetencia });
        }

        [HttpGet]
        public ActionResult GetLancamentosConciliacao(Guid extratoBancarioId)
        {
            var extrato = _ExtratoBancarioApp.GetById(extratoBancarioId);
            var lancamentos = _LancamentoAppService.GetLancamentosSugeridosParaConciliacao(extrato);
            var conciliacao = new ConciliacaoViewModel { ExtratoBancario = extrato, Lancamentos = lancamentos };
            return PartialView("_LancamentosConciliacao", conciliacao);
        }

        public JsonResult ConciliarLancamento(string lancamentoJson, Guid extratoBancarioId)
        {
            GrupoLancamentoViewModel grupoLancamento = null;
            LancamentoViewModel lancamento = null;

            try
            {
                grupoLancamento = JsonConvert.DeserializeObject<GrupoLancamentoViewModel>(lancamentoJson);
            }
            catch(JsonSerializationException)
            {
                lancamento = JsonConvert.DeserializeObject<LancamentoViewModel>(lancamentoJson);
            }

            var jsonResult = new JsonResult()
            {
                Data = new { status = "Ok" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            try
            {
                if (grupoLancamento == null)
                {
                    lancamento.ExtratoBancarioId = extratoBancarioId;
                    lancamento.Pago = true;

                    if (lancamento.Fake)
                        _LancamentoAppService.Add(lancamento);
                    else
                        _LancamentoAppService.Update(lancamento);
                }
                else
                {
                    grupoLancamento.ExtratoBancarioId = extratoBancarioId;
                    _GrupoLancamentoAppService.Update(grupoLancamento);
                }
            }
            catch(Exception ex)
            {
                jsonResult.Data = new { status = "Nok" };
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return jsonResult;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoverSelecionados(IEnumerable<ExtratoBancarioViewModel> extratos, DateTime MesAnoCompetencia)
        {
            _ExtratoBancarioApp.RemoveAll(extratos.Where(ex => ex.Selecionado));
            var cookieContaId = Util.GetCookieContaId(Request, Response);
            var ContaIdFiltro = Guid.Parse(cookieContaId.Value.ToString());
            return RedirectToAction("Index", new { ContaIdFiltro, MesAnoCompetencia });
        }

        private void SetSelectLists()
        {
            ViewBag.Contas = new SelectList(_ContaApp.GetAll(), "ContaId", "Descricao");
        }
    }
}
