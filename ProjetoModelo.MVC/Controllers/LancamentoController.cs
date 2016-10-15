using System;
using System.Linq;
using System.Web.Mvc;
using Moneta.Application.Interfaces;
using Moneta.Application.ViewModels;
using Moneta.MVC.DataAnnotation;

namespace Moneta.MVC.Controllers
{
    public class LancamentosController : Controller
    {
        private readonly ILancamentoAppService _LancamentoApp;
        private readonly ICategoriaAppService _CategoriaApp;
        private readonly IContaAppService _ContaApp;

        public LancamentosController(
            ILancamentoAppService LancamentoApp,
            ICategoriaAppService CategoriaApp,
            IContaAppService ContaApp)
        {
            _LancamentoApp = LancamentoApp;
            _CategoriaApp = CategoriaApp;
            _ContaApp = ContaApp;
        }

        // GET: Lancamento
        public ViewResult Index(LancamentosDoMesViewModel lancamentos)
        {
            if (TempData["ViewData"] != null)
            {
                ViewData = (ViewDataDictionary)TempData["ViewData"];
                lancamentos = (LancamentosDoMesViewModel)ViewData.Model;
            }

            /*
             * Mudar quando colocar view por mes
             * */
            lancamentos = _LancamentoApp.GetLancamentosDoMes(DateTime.Now.Month, DateTime.Now.Year, (Guid)lancamentos.ContaIdFiltro);

            SetSelectLists();

            return View(lancamentos);
        }

        public ActionResult TrocarPago(Guid lancamentoId, Guid contaIdFiltro)
        {
            var lancamento = _LancamentoApp.GetByIdReadOnly(lancamentoId);
            lancamento.Pago = !lancamento.Pago;
            _LancamentoApp.Update(lancamento);
            var lancamentos = new LancamentosDoMesViewModel();
            lancamentos.ContaIdFiltro = contaIdFiltro;

            ViewData.Model = lancamentos;
            TempData["ViewData"] = ViewData;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Pesquisar")]
        public ActionResult Pesquisar(LancamentosDoMesViewModel lancamentos)
        {
            ViewData.Model = lancamentos;
            TempData["ViewData"] = ViewData;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "AdicionarDespesa")]
        public ActionResult AdicionarDespesa(LancamentosDoMesViewModel lancamentos)
        {
            if (ModelState.IsValid)
            {
                lancamentos.NovaTransacao = TipoTransacao.Despesa;
                return RedirectToAction("Create", lancamentos);
            }

            ViewData.Model = lancamentos;
            TempData["ViewData"] = ViewData;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "AdicionarReceita")]
        public ActionResult AdicionarReceita(LancamentosDoMesViewModel lancamentos)
        {
            if (ModelState.IsValid)
            {
                lancamentos.NovaTransacao = TipoTransacao.Receita;
                return RedirectToAction("Create", lancamentos);
            }

            ViewData.Model = lancamentos;
            TempData["ViewData"] = ViewData;
            return RedirectToAction("Index");
        }

        // GET: Lancamento/Details/5
        public ActionResult Details(Guid id)
        {
            var LancamentoViewModel = _LancamentoApp.GetById(id);

            return View(LancamentoViewModel);
        }

        private void SetSelectLists()
        {
            ViewBag.Contas = new SelectList(_ContaApp.GetAll(), "ContaId", "Descricao");
            ViewBag.Categorias = new SelectList(_CategoriaApp.GetAll(), "CategoriaId", "Descricao");
        }

        // GET: Lancamento/Create
        public ActionResult Create(LancamentosDoMesViewModel lancamentos)
        {
            if (ModelState.IsValid)
            {
                var novoLancamento = new LancamentoViewModel();
                novoLancamento.Conta = _ContaApp.GetById(lancamentos.ContaIdFiltro);
                novoLancamento.ContaId = lancamentos.ContaIdFiltro;
                novoLancamento.Transacao = lancamentos.NovaTransacao;
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

                return RedirectToAction("Index", new { contaIdFiltro = lancamento.ContaId });
            }

            lancamento.Conta = _ContaApp.GetById(lancamento.ContaId);
            return View(lancamento);
        }

        // GET: Lancamento/Edit/5
        public ActionResult Edit(Guid id)
        {
            var LancamentoViewModel = _LancamentoApp.GetById(id);
            SetSelectLists();
            return View(LancamentoViewModel);
        }

        // POST: Lancamento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LancamentoViewModel lancamento)
        {
            if (ModelState.IsValid)
            {
                _LancamentoApp.Update(lancamento);

                return RedirectToAction("Index", new { contaIdFiltro = lancamento.ContaId });
            }

            return View(lancamento);
        }

        // GET: Lancamento/Delete/5
        public ActionResult Delete(Guid id)
        {
            var LancamentoViewModel = _LancamentoApp.GetById(id);

            return View(LancamentoViewModel);
        }

        // POST: Lancamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var lancamento = _LancamentoApp.GetByIdReadOnly(id);
            _LancamentoApp.Remove(lancamento);

            return RedirectToAction("Index", new { contaIdFiltro = lancamento.ContaId });
        }
    }
}
