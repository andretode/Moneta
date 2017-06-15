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
    [Authorize(Roles = "UsuarioPlanoBasico")]
    public class LancamentosController : Controller
    {
        private readonly ILancamentoParceladoAppService _LancamentoParceladoApp;
        private readonly ILancamentoAppService _LancamentoApp;
        private readonly ICategoriaAppService _CategoriaApp;
        private readonly IContaAppService _ContaApp;
        private readonly IGrupoLancamentoAppService _GrupoLancamentoApp;
        private readonly IExtratoBancarioAppService _ExtratoBancarioApp;

        public LancamentosController(
            ILancamentoParceladoAppService LancamentoParceladoApp,
            ILancamentoAppService LancamentoApp,
            ICategoriaAppService CategoriaApp,
            IContaAppService ContaApp,
            IGrupoLancamentoAppService GrupoLancamentoApp,
            IExtratoBancarioAppService ExtratoBancarioApp)
        {
            _LancamentoParceladoApp = LancamentoParceladoApp;
            _LancamentoApp = LancamentoApp;
            _CategoriaApp = CategoriaApp;
            _ContaApp = ContaApp;
            _GrupoLancamentoApp = GrupoLancamentoApp;
            _ExtratoBancarioApp = ExtratoBancarioApp;
        }

        // GET: Lancamento
        public ViewResult Index(LancamentosDoMesViewModel lancamentos)
        {
            var cookieContaId = Util.GetCookieContaId(Request, Response);
            var contaIdFiltroTemp = Guid.Parse(cookieContaId.Value.ToString());

            if (lancamentos.MesAnoCompetencia == DateTime.MinValue)
                lancamentos.MesAnoCompetencia = DateTime.Now;

            DateTime mesAnoCompetenciaTemp = lancamentos.MesAnoCompetencia;
            lancamentos.ContaIdFiltro = contaIdFiltroTemp;

            lancamentos = _LancamentoApp.GetLancamentosDoMes(lancamentos);
            lancamentos.MesAnoCompetencia = mesAnoCompetenciaTemp;
            lancamentos.contaIdFiltro = contaIdFiltroTemp;

            SetSelectLists();

            return View(lancamentos);
        }

        public ViewResult AlterarMes(DateTime MesAnoCompetencia, int addMonths)
        {
            var lancamentosDoMes = new LancamentosDoMesViewModel();
            var cookieContaId = Util.GetCookieContaId(Request, Response);
            var ContaIdFiltro = Guid.Parse(cookieContaId.Value.ToString());
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

            if (lancamento.GrupoLancamentoId == null)
            {
                lancamento.Pago = !lancamento.Pago;
                if (lancamento.Fake)
                    _LancamentoApp.Add(lancamento);
                else
                    _LancamentoApp.Update(lancamento);
            }
            else
            {
                var grupoLancamento = _GrupoLancamentoApp.GetByIdReadOnly((Guid)lancamento.GrupoLancamentoId);
                _LancamentoApp.TrocarPago(grupoLancamento.Lancamentos);
            }

            return RedirectToAction("Index", new { ContaIdFiltro = lancamento.ContaId, MesAnoCompetencia = lancamento.DataVencimento });
        
        }

        [HttpGet]
        public ActionResult Desconciliar(string jsonLancamento)
        {
            var lancamento = JsonConvert.DeserializeObject<LancamentoViewModel>(jsonLancamento);

            if (lancamento.GrupoLancamentoId == null)
            {
                lancamento.ExtratoBancarioId = null;
                _LancamentoApp.Update(lancamento);
            }
            else
            {
                var grupoLancamento = _GrupoLancamentoApp.GetByIdReadOnly((Guid)lancamento.GrupoLancamentoId);
                grupoLancamento.ExtratoBancarioId = null;
                _GrupoLancamentoApp.Update(grupoLancamento);
            }

            return RedirectToAction("Index", new { ContaIdFiltro = lancamento.ContaId, MesAnoCompetencia = lancamento.DataVencimento });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoverSelecionados(IEnumerable<LancamentoAgrupadoViewModel> lancamentosAgrupados, DateTime MesAnoCompetencia)
        {
            var lancamentosSelecionados = lancamentosAgrupados
                .Where(agrupado => agrupado.Lancamentos.First().Selecionado)
                .Select(agrupados => agrupados.Lancamentos.First());
            _LancamentoApp.RemoveAll(lancamentosSelecionados);
            var cookieContaId = Util.GetCookieContaId(Request, Response);
            var ContaIdFiltro = Guid.Parse(cookieContaId.Value.ToString());
            return RedirectToAction("Index", new { ContaIdFiltro, MesAnoCompetencia });
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Pesquisar")]
        public ActionResult Pesquisar(LancamentosDoMesViewModel lancamentos)
        {
            return RedirectToAction("Index", new { MesAnoCompetencia = lancamentos.MesAnoCompetencia,
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

            return RedirectToAction("Index", new { MesAnoCompetencia = lancamentos.MesAnoCompetencia });
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

            return RedirectToAction("Index", new { MesAnoCompetencia = lancamentos.MesAnoCompetencia });
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

            return RedirectToAction("Index", new { MesAnoCompetencia = lancamentos.MesAnoCompetencia });
        }

        // GET: Lancamento/Details/5
        public ActionResult Details(string jsonLancamento)
        {
            var lancamento = JsonConvert.DeserializeObject<LancamentoViewModel>(jsonLancamento);
            RecomporLancamento(lancamento);

            return View(lancamento);
        }

        public ActionResult DetailsLancamentoAgrupado(Guid id)
        {
            var lancamento = _LancamentoApp.GetById(id);

            return View("Details", lancamento);
        }

        public ActionResult DetailsFromExtratoView(Guid id)
        {
            var lancamento = _LancamentoApp.GetById(id);
            RecomporLancamento(lancamento);

            return View("Details", lancamento);
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
                novoLancamento.DataVencimento = DateTime.Now;

                if (lancamentos.GrupoLancamentoId != null)
                {
                    novoLancamento.GrupoLancamentoId = lancamentos.GrupoLancamentoId;
                    novoLancamento.GrupoLancamento = _GrupoLancamentoApp.GetById((Guid)lancamentos.GrupoLancamentoId);
                }

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
            Util.TratarLancamentoValorHtml5Number(lancamento, ModelState);

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

                if (lancamento.GrupoLancamentoId != null)
                    return RedirectToAction("Details", "GrupoLancamentos", new { id = lancamento.GrupoLancamentoId });
                else
                    return RedirectToAction("Index", new { MesAnoCompetencia = lancamento.DataVencimento });
            }

            lancamento.Conta = _ContaApp.GetById(lancamento.ContaId);
            return View(lancamento);
        }

        public ActionResult CreateFromExtrato(LancamentoViewModel lancamento, bool nada=true)
        {
            SetSelectLists();
            lancamento.Conta = _ContaApp.GetById(lancamento.ContaId);
            return View(lancamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFromExtrato(LancamentoViewModel lancamento)
        {
            SetSelectLists();
            Util.TratarLancamentoValorHtml5Number(lancamento, ModelState);

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

                return RedirectToAction("Index", "ExtratosBancarios",  new { MesAnoCompetencia = lancamento.DataVencimento });
            }

            lancamento.Conta = _ContaApp.GetById(lancamento.ContaId);
            return View(lancamento);
        }

        public JsonResult EditarCategoria(Guid lancamentoId, Guid categoriaId)
        {
            var jsonResult = new JsonResult()
            {
                Data = new { status = "Ok" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            try
            {
                var lancamento = _LancamentoApp.GetByIdReadOnly(lancamentoId);
                lancamento.CategoriaId = categoriaId;
                _LancamentoApp.Update(lancamento);
            }
            catch(Exception ex)
            {
                jsonResult.Data = new { status = "Nok", erro = ex.Message };
            }

            return jsonResult;
        }

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
            Util.TratarLancamentoValorHtml5Number(lancamento, ModelState);

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


                return RedirectToAction("Index", new { MesAnoCompetencia = lancamento.DataVencimento });
            }
            SetSelectLists();
            return View(lancamento);
        }

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
            var grupoLancamentoId = lancamento.GrupoLancamentoId;
            if (ModelState.IsValid)
            {
                _LancamentoApp.Remove(lancamento);

                if (grupoLancamentoId != null)
                    return RedirectToAction("Details", "GrupoLancamentos", new { id = lancamento.GrupoLancamentoId });
                else
                    return RedirectToAction("Index", new { MesAnoCompetencia = lancamento.DataVencimento });
            }

            return View(lancamento);
        }


        #region Metodos Privados
        private void RecomporLancamento(LancamentoViewModel lancamento)
        {
            lancamento.Conta = _ContaApp.GetById(lancamento.ContaId);
            lancamento.Categoria = _CategoriaApp.GetById(lancamento.CategoriaId);
            if (lancamento.ExtratoBancarioId != null)
                lancamento.ExtratoBancario = _ExtratoBancarioApp.GetById((Guid)lancamento.ExtratoBancarioId);
            if (lancamento.LancamentoParceladoId != null)
                lancamento.LancamentoParcelado = _LancamentoParceladoApp.GetById((Guid)lancamento.LancamentoParceladoId);
        }

        private void SetSelectLists()
        {
            ViewBag.Contas = new SelectList(_ContaApp.GetAll(), "ContaId", "Descricao");
            ViewBag.Categorias = new SelectList(_CategoriaApp.GetAll(), "CategoriaId", "Descricao");
        }

        #endregion

    }
}
