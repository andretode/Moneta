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
        public ViewResult Index(LancamentosViewModel lancamentos, Guid? contaIdFiltro = null)
        {
            if (contaIdFiltro == null)
                contaIdFiltro = lancamentos.ContaIdFiltro;

            lancamentos.Lancamentos = _LancamentoApp.GetAll().Where(l => 
                l.ContaId == contaIdFiltro).OrderBy(l => l.DataVencimento).ThenBy(l => l.Descricao);
            
            /*
             * TROCA ISTO QUANDO COLOCAR O FILTRO POR MES
             **/
            if (contaIdFiltro != null)
                lancamentos.SaldoDoMes = _LancamentoApp.SaldoDoMes(DateTime.Now.Month, (Guid)contaIdFiltro);

            SetSelectLists();

            return View(lancamentos);
        }

        public ActionResult TrocarPago(Guid id, Guid contaIdFiltro)
        {
            var lancamento = _LancamentoApp.GetByIdReadOnly(id);
            lancamento.Pago = !lancamento.Pago;
            _LancamentoApp.Update(lancamento);
            var lancamentos = new LancamentosViewModel();
            lancamentos.ContaIdFiltro = contaIdFiltro;

            return RedirectToAction("Index", new { contaIdFiltro });
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Pesquisar")]
        public ActionResult Pesquisar(LancamentosViewModel lancamentos, string pesquisa = "")
        {
            return RedirectToAction("Index", lancamentos);
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "AdicionarDespesa")]
        public ActionResult AdicionarDespesa(LancamentosViewModel lancamentos)
        {
            if (ModelState.IsValid)
            {
                lancamentos.NovaTransacao = TipoTransacao.Despesa;
                return RedirectToAction("Create", lancamentos);
            }

            return RedirectToAction("Index", lancamentos);
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "RealizarLancamento")]
        public ActionResult CriarLancamento(LancamentosViewModel lancamentos)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Create", lancamentos);
            }

            return RedirectToAction("Index", lancamentos);
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
        public ActionResult Create(LancamentosViewModel lancamentos)
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
