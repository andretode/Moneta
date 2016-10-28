using Moneta.Application.Interfaces;
using Moneta.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moneta.MVC.Controllers
{
    public class TransferenciasController : Controller
    {
        private readonly ILancamentoAppService _LancamentoApp;
        private readonly ICategoriaAppService _CategoriaApp;
        private readonly IContaAppService _ContaApp;

        public TransferenciasController (
            ILancamentoAppService LancamentoApp,
            ICategoriaAppService CategoriaApp,
            IContaAppService ContaApp)
        {
            _LancamentoApp = LancamentoApp;
            _CategoriaApp = CategoriaApp;
            _ContaApp = ContaApp;
        }
        
        
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Details(Guid id)
        {
            var lancamento = _LancamentoApp.GetById(id);

            return View(lancamento);
        }

        public ActionResult Edit(Guid id)
        {
            var lancamento = _LancamentoApp.GetById(id);
            SetSelectLists();
            return View(lancamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LancamentoViewModel lancamento)
        {
            if (ModelState.IsValid)
            {
                _LancamentoApp.Update(lancamento);
                
                return RedirectToAction("Index", "Lancamentos", new { contaIdFiltro = lancamento.ContaId, MesAnoCompetencia = lancamento.DataVencimento });
            }

            return View(lancamento);
        }

        private void SetSelectLists()
        {
            ViewBag.Contas = new SelectList(_ContaApp.GetAll(), "ContaId", "Descricao");
        }

    }
}