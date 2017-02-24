using Moneta.Application.Interfaces;
using Moneta.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moneta.MVC.Controllers
{
    public class GrupoLancamentosController : Controller
    {
        private readonly IGrupoLancamentoAppService _grupoLancamentoApp;
        private readonly IContaAppService _contaApp;

        public GrupoLancamentosController(
            IGrupoLancamentoAppService grupoLancamentoApp,
            IContaAppService contaApp)
        {
            _grupoLancamentoApp = grupoLancamentoApp;
            _contaApp = contaApp;
        }

        // GET: GrupoLancamentos
        public ActionResult Index()
        {
            var gruposLancamento = _grupoLancamentoApp.GetAll().OrderBy(c => c.Descricao);
            return View(gruposLancamento);
        }

        // GET: GrupoLancamentos/Details/5
        public ActionResult Details(Guid id)
        {
            var grupoLancamento = _grupoLancamentoApp.GetById(id);
            return View(grupoLancamento);
        }

        // GET: GrupoLancamentos/Create
        public ActionResult Create()
        {
            SetSelectLists();
            return View();
        }

        // POST: GrupoLancamentos/Create
        [HttpPost]
        public ActionResult Create(GrupoLancamentoViewModel grupoLancamento)
        {
            if (ModelState.IsValid)
            {
                var result = _grupoLancamentoApp.Add(grupoLancamento);

                if (!result.IsValid)
                {
                    foreach (var validationAppError in result.Erros)
                    {
                        ModelState.AddModelError(string.Empty, validationAppError.Message);
                    }
                    return View(grupoLancamento);
                }

                return RedirectToAction("Index");
            }

            SetSelectLists();
            return View(grupoLancamento);
        }

        // GET: GrupoLancamentos/Edit/5
        public ActionResult Edit(Guid id)
        {
            var grupoLancamento = _grupoLancamentoApp.GetById(id);
            SetSelectLists();
            return View(grupoLancamento);
        }

        // POST: GrupoLancamentos/Edit/5
        [HttpPost]
        public ActionResult Edit(GrupoLancamentoViewModel grupoLancamento)
        {
            if (ModelState.IsValid)
            {
                _grupoLancamentoApp.Update(grupoLancamento);

                return RedirectToAction("Index");
            }

            SetSelectLists();
            return View(grupoLancamento);
        }

        // GET: GrupoLancamentos/Delete/5
        public ActionResult Delete(Guid id)
        {
            var grupoLancamentoViewModel = _grupoLancamentoApp.GetAllReadOnly().Where(c => c.GrupoLancamentoId == id).First();
            _grupoLancamentoApp.Remove(grupoLancamentoViewModel);
            return RedirectToAction("Index");
        }

        private void SetSelectLists()
        {
            ViewBag.Contas = new SelectList(_contaApp.GetAll(), "ContaId", "Descricao");
        }
    }
}
