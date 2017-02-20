using System;
using System.Linq;
using System.Web.Mvc;
using Moneta.Application.Interfaces;
using Moneta.Application.ViewModels;
using Moneta.MVC.DataAnnotation;
using Moneta.Infra.CrossCutting.Enums;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Threading;

namespace Moneta.MVC.Controllers
{
    public class LancamentosController : Controller
    {
        private readonly ILancamentoParceladoAppService _LancamentoParceladoApp;
        private readonly ILancamentoAppService _LancamentoApp;
        private readonly ICategoriaAppService _CategoriaApp;
        private readonly IContaAppService _ContaApp;

        public LancamentosController(
            ILancamentoParceladoAppService LancamentoParceladoApp,
            ILancamentoAppService LancamentoApp,
            ICategoriaAppService CategoriaApp,
            IContaAppService ContaApp)
        {
            _LancamentoParceladoApp = LancamentoParceladoApp;
            _LancamentoApp = LancamentoApp;
            _CategoriaApp = CategoriaApp;
            _ContaApp = ContaApp;
        }

        // GET: Lancamento
        public ViewResult Index(LancamentosDoMesViewModel lancamentos)
        {
            if (lancamentos.MesAnoCompetencia == DateTime.MinValue)
                lancamentos.MesAnoCompetencia = DateTime.Now;

            DateTime mesAnoCompetenciaTemp = lancamentos.MesAnoCompetencia;
            Guid contaIdFiltroTemp = (lancamentos.ContaIdFiltro == null ? Guid.Empty : (Guid)lancamentos.ContaIdFiltro);

            lancamentos = _LancamentoApp.GetLancamentosDoMes(lancamentos);
            lancamentos.MesAnoCompetencia = mesAnoCompetenciaTemp;
            lancamentos.ContaIdFiltro = contaIdFiltroTemp;

            SetSelectLists();

            return View(lancamentos);
        }

        public ActionResult GraficoSaldoDoMes(Guid? ContaIdFiltro)
        {
            ContaIdFiltro = (ContaIdFiltro == null ? Guid.Empty : (Guid)ContaIdFiltro);

            var lancamentosDoMes = new LancamentosDoMesViewModel();
            lancamentosDoMes.ContaIdFiltro = (Guid)ContaIdFiltro;
            lancamentosDoMes.MesAnoCompetencia = DateTime.Now;
            var listaSaldoDoMesPorDia = _LancamentoApp.GetSaldoDoMesPorDia(lancamentosDoMes, false);

            SetSelectLists();
            return View(listaSaldoDoMesPorDia);
        }

        public ViewResult AlterarMes(DateTime MesAnoCompetencia, Guid ContaIdFiltro, int addMonths)
        {
            var lancamentosDoMes = new LancamentosDoMesViewModel();
            lancamentosDoMes.ContaIdFiltro = ContaIdFiltro;
            MesAnoCompetencia = MesAnoCompetencia.AddMonths(addMonths);
            lancamentosDoMes.MesAnoCompetencia = MesAnoCompetencia;
            var lancamentos = _LancamentoApp.GetLancamentosDoMes(lancamentosDoMes);
            lancamentos.MesAnoCompetencia = MesAnoCompetencia;
            lancamentos.ContaIdFiltro = ContaIdFiltro;

            SetSelectLists();
            return View("Index", lancamentos);
        }

        [HttpGet]
        public ActionResult TrocarPago(string jsonLancamento)
        {
            var lancamento = JsonConvert.DeserializeObject<LancamentoViewModel>(jsonLancamento);
            lancamento.Pago = !lancamento.Pago;

            if(lancamento.Fake)
                _LancamentoApp.Add(lancamento);
            else
                _LancamentoApp.Update(lancamento);

            return RedirectToAction("Index", new { ContaIdFiltro = lancamento.ContaId, MesAnoCompetencia = lancamento.DataVencimento });
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Pesquisar")]
        public ActionResult Pesquisar(LancamentosDoMesViewModel lancamentos)
        {
            return RedirectToAction("Index", new { ContaIdFiltro = lancamentos.ContaIdFiltro, MesAnoCompetencia = lancamentos.MesAnoCompetencia,
                PesquisarDescricao = lancamentos.PesquisarDescricao });
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "AdicionarDespesa")]
        public ActionResult AdicionarDespesa(LancamentosDoMesViewModel lancamentos)
        {
            if (ModelState.IsValid)
            {
                lancamentos.NovaTransacao = TipoTransacaoEnum.Despesa;
                return RedirectToAction("Create", lancamentos);
            }

            return RedirectToAction("Index", new { ContaIdFiltro = lancamentos.ContaIdFiltro, MesAnoCompetencia = lancamentos.MesAnoCompetencia });
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "AdicionarReceita")]
        public ActionResult AdicionarReceita(LancamentosDoMesViewModel lancamentos)
        {
            if (ModelState.IsValid)
            {
                lancamentos.NovaTransacao = TipoTransacaoEnum.Receita;
                return RedirectToAction("Create", lancamentos);
            }

            return RedirectToAction("Index", new { ContaIdFiltro = lancamentos.ContaIdFiltro, MesAnoCompetencia = lancamentos.MesAnoCompetencia });
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "RealizarTransferencia")]
        public ActionResult RealizarTransferencia(LancamentosDoMesViewModel lancamentos)
        {
            if (ModelState.IsValid)
            {
                lancamentos.NovaTransacao = TipoTransacaoEnum.Transferencia;
                return RedirectToAction("Create", "Transferencias", lancamentos);
            }

            return RedirectToAction("Index", new { ContaIdFiltro = lancamentos.ContaIdFiltro, MesAnoCompetencia = lancamentos.MesAnoCompetencia });
        }

        // GET: Lancamento/Details/5
        public ActionResult Details(string jsonLancamento)
        {
            var lancamento = JsonConvert.DeserializeObject<LancamentoViewModel>(jsonLancamento);
            RecomporLancamento(lancamento);

            return View(lancamento);
        }

        // GET: Lancamento/Create
        public ActionResult Create(LancamentosDoMesViewModel lancamentos)
        {
            if (ModelState.IsValid)
            {
                var novoLancamento = new LancamentoViewModel(lancamentos.MesAnoCompetencia);
                novoLancamento.Conta = _ContaApp.GetById((Guid)lancamentos.ContaIdFiltro);
                novoLancamento.ContaId = (Guid)lancamentos.ContaIdFiltro;
                novoLancamento.TipoDeTransacao = lancamentos.NovaTransacao;
                SetSelectLists();
                return View(novoLancamento);
            }
            return RedirectToAction("Index", new { lancamentos });
        }

        // POST: Lancamento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LancamentoViewModel lancamento)
        {
            SetSelectLists();

            if (ModelState.IsValid)
            {
                var result = _LancamentoApp.Add(lancamento);

                if (!result.IsValid)
                {
                    foreach (var validationAppError in result.Erros)
                    {
                        ModelState.AddModelError(string.Empty, validationAppError.Message);
                    }
                    return View(lancamento);
                }

                return RedirectToAction("Index", new { contaIdFiltro = lancamento.ContaId, MesAnoCompetencia = lancamento.DataVencimento });
            }

            lancamento.Conta = _ContaApp.GetById(lancamento.ContaId);
            return View(lancamento);
        }

        // GET: Lancamento/Edit/5
        public ActionResult Edit(string jsonLancamento)
        {
            var lancamento = JsonConvert.DeserializeObject<LancamentoViewModel>(jsonLancamento);
            RecomporLancamento(lancamento);

            SetSelectLists();
            return View(lancamento);
        }

        // POST: Lancamento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LancamentoViewModel lancamento)
        {
            if (ModelState.IsValid)
            {
                if (lancamento.LancamentoParcelado == null || 
                    lancamento.LancamentoParcelado.TipoDeAlteracaoDaRepeticao == TipoDeAlteracaoDaRepeticaoEnum.AlterarApenasEste)
                {
                    if (lancamento.Fake)
                        _LancamentoApp.Add(lancamento);
                    else
                        _LancamentoApp.Update(lancamento);
                }
                else
                {
                    _LancamentoApp.UpdateEmSerie(lancamento);
                }


                return RedirectToAction("Index", new { contaIdFiltro = lancamento.ContaId, MesAnoCompetencia = lancamento.DataVencimento });
            }
            SetSelectLists();
            return View(lancamento);
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

            if (caminhoOfx!="")
                _LancamentoApp.ImportarOfx(caminhoOfx, contaId);
            else
                ModelState.AddModelError(string.Empty, "O arquivo OFX não foi encontrado");

            return RedirectToAction("Index");
        }

        // GET: Lancamento/Delete/5
        public ActionResult Delete(string jsonLancamento)
        {
            var lancamento = JsonConvert.DeserializeObject<LancamentoViewModel>(jsonLancamento);

            return View(lancamento);
        }

        // POST: Lancamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(LancamentoViewModel lancamento)
        {
            if (ModelState.IsValid)
            {
                var tipoDeAlteracaoDaRepeticao = lancamento.LancamentoParcelado.TipoDeAlteracaoDaRepeticao;
                lancamento.LancamentoParcelado = _LancamentoParceladoApp.GetByIdReadOnly((Guid)lancamento.LancamentoParceladoId);
                lancamento.LancamentoParcelado.TipoDeAlteracaoDaRepeticao = tipoDeAlteracaoDaRepeticao;
                
                _LancamentoApp.RemoveEmSerie(lancamento);

                return RedirectToAction("Index", new { contaIdFiltro = lancamento.ContaId, MesAnoCompetencia = lancamento.DataVencimento });
            }
            return View(lancamento);
        }


        #region Metodos Privados
        private void RecomporLancamento(LancamentoViewModel lancamento)
        {
            lancamento.Conta = _ContaApp.GetById(lancamento.ContaId);
            lancamento.Categoria = _CategoriaApp.GetById(lancamento.CategoriaId);
            if (lancamento.LancamentoParceladoId != null)
                lancamento.LancamentoParcelado = _LancamentoParceladoApp.GetById((Guid)lancamento.LancamentoParceladoId);
            _LancamentoApp.AjustarLancamentoParaExibir(lancamento);
        }

        private void SetSelectLists()
        {
            ViewBag.Contas = new SelectList(_ContaApp.GetAll(), "ContaId", "Descricao");
            ViewBag.Categorias = new SelectList(_CategoriaApp.GetAll(), "CategoriaId", "Descricao");
        }

        #endregion

    }
}
