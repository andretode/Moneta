using System;
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
    public class ExtratosBancariosController : Controller
    {
        private readonly IExtratoBancarioAppService _ExtratoBancarioApp;
        private readonly ILancamentoAppService _LancamentoAppService;
        private readonly IContaAppService _ContaApp;

        public ExtratosBancariosController(
            IExtratoBancarioAppService extratoBancarioApp,
            ILancamentoAppService lancamentoAppService,
            IContaAppService contaApp)
        {
            _ExtratoBancarioApp = extratoBancarioApp;
            _LancamentoAppService = lancamentoAppService;
            _ContaApp = contaApp;
        }

        public ViewResult Index(string pesquisa, int page = 0, int quant = -1)
        {
            var extratoBancarioViewModel = _ExtratoBancarioApp.GetAll().OrderBy(c => c.DataCompensacao);
            ViewBag.Pesquisa = pesquisa;
            ViewBag.quantidadeImportada = quant;

            return View(extratoBancarioViewModel);
        }

        public ViewResult AlterarMes(DateTime mesAnoCompetencia, Guid contaIdFiltro, int addMonths)
        {
            mesAnoCompetencia = mesAnoCompetencia.AddMonths(addMonths);
            var extratos = _ExtratoBancarioApp.GetAll().Where(e => e.DataCompensacao.Month == mesAnoCompetencia.Month
                && e.DataCompensacao.Year == mesAnoCompetencia.Year);

            ViewBag.quantidadeImportada = -1;
            SetSelectLists();
            return View("Index", extratos);
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

        public ActionResult ImportarOfx()
        {
            SetSelectLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportarOfx(string arquivoOfx)
        {
            int quantidadeImportada = 0;
            string caminhoOfx = "";
            Guid contaId = Guid.Parse(Request.Form.Get("contaIdFiltro"));

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
                return View();
            }

            try
            {
                quantidadeImportada = _ExtratoBancarioApp.ImportarOfx(caminhoOfx, contaId);
            }
            catch (FormatException fx)
            {
                ModelState.AddModelError(string.Empty, "O arquivo enviado tem um formato OFX desconhecido. Detalhes técnicos do erro: " + fx.Message);
                SetSelectLists();
                return View();
            }
            catch (XmlException xe)
            {
                ModelState.AddModelError(string.Empty, "O arquivo enviado tem um formato OFX fora do padrão. Detalhes técnicos do erro: " + xe.Message);
                SetSelectLists();
                return View();
            }

            return RedirectToAction("Index", new { quant = quantidadeImportada });
        }

        public ActionResult Delete(Guid id)
        {
            var extratoBancario = _ExtratoBancarioApp.GetAllReadOnly().Where(c => c.ExtratoBancarioId == id).First();
            _ExtratoBancarioApp.Remove(extratoBancario);

            return RedirectToAction("Index");
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
            var lancamento = JsonConvert.DeserializeObject<LancamentoViewModel>(lancamentoJson);
            lancamento.ExtratoBancarioId = extratoBancarioId;

            if (lancamento.Fake)
                _LancamentoAppService.Add(lancamento);
            else
                _LancamentoAppService.Update(lancamento);

            return new JsonResult()
            {
                Data = new { status = "Ok" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoverSelecionados(IEnumerable<ExtratoBancarioViewModel> extratos)
        {
            _ExtratoBancarioApp.RemoveAll(extratos.Where(ex => ex.Selecionado));
            return RedirectToAction("Index");
        }

        private void SetSelectLists()
        {
            ViewBag.Contas = new SelectList(_ContaApp.GetAll(), "ContaId", "Descricao");
        }
    }
}
