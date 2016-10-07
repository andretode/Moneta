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
        public ViewResult Index(LancamentosViewModel lancamentos, string pesquisa = "", int page = 0)
        {
            lancamentos.Lancamentos = _LancamentoApp.GetAll().Where(l => l.ContaId == lancamentos.ContaIdFiltro);
            SetSelectLists();

            return View(lancamentos);
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Pesquisar")]
        public ActionResult Pesquisar(LancamentosViewModel lancamentos, string pesquisa = "")
        {
            return RedirectToAction("Index", lancamentos);
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "CriarLancamento")]
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
                SetSelectLists();
                return View(novoLancamento);
            }
            return RedirectToAction("Index");
        }

        // POST: Lancamento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LancamentoViewModel LancamentoEndereco)
        {
            SetSelectLists();
            if (ModelState.IsValid)
            {
                var result = _LancamentoApp.Add(LancamentoEndereco);

                if (!result.IsValid)
                {
                    foreach (var validationAppError in result.Erros)
                    {
                        ModelState.AddModelError(string.Empty, validationAppError.Message);
                    }
                    return View(LancamentoEndereco);
                }

                return RedirectToAction("Index");
            }
     
            return View(LancamentoEndereco);
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
        public ActionResult Edit(LancamentoViewModel LancamentoViewModel)
        {
            if (ModelState.IsValid)
            {
                _LancamentoApp.Update(LancamentoViewModel);

                return RedirectToAction("Index");
            }

            return View(LancamentoViewModel);
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
            var Lancamento = _LancamentoApp.GetAllReadOnly().Where(c => c.LancamentoId == id).First(); //_LancamentoApp.GetById(id);
            _LancamentoApp.Remove(Lancamento);

            return RedirectToAction("Index");
        }
    }
}
