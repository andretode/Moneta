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

        public ActionResult CreateFromExtrato(TransferenciaViewModel transferencia)
        {
            SetSelectLists();
            return View(transferencia);
        }

        public ActionResult Create(LancamentosDoMesViewModel lancamentos)
        {
            if (ModelState.IsValid)
            {
                var novaTransferencia = new TransferenciaViewModel(lancamentos.MesAnoCompetencia);
                novaTransferencia.LancamentoOrigem.Conta = _ContaApp.GetById((Guid)lancamentos.ContaIdFiltro);
                novaTransferencia.LancamentoOrigem.CategoriaId = _CategoriaApp.GetAll().Where(c => c.Descricao == CategoriaViewModel.Nenhum).FirstOrDefault().CategoriaId;
                SetSelectLists((Guid)lancamentos.ContaIdFiltro);
                return View(novaTransferencia);
            }
            return RedirectToAction("Index", new { lancamentos });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TransferenciaViewModel transferencia)
        {
            SetSelectLists(transferencia.LancamentoOrigem.ContaId);
            if (ModelState.IsValid)
            {
                transferencia.LancamentoOrigem.ContaId = transferencia.LancamentoOrigem.Conta.ContaId;
                var result = _LancamentoApp.AddTransferencia(transferencia);

                if (!result.IsValid)
                {
                    foreach (var validationAppError in result.Erros)
                    {
                        ModelState.AddModelError(string.Empty, validationAppError.Message);
                    }
                    return View(transferencia);
                }

                return RedirectToAction("Index", "Lancamentos", new { contaIdFiltro = transferencia.LancamentoOrigem.ContaId, MesAnoCompetencia = transferencia.LancamentoOrigem.DataVencimento });
            }

            transferencia.LancamentoOrigem.Conta = _ContaApp.GetById(transferencia.LancamentoOrigem.ContaId);
            return View(transferencia);
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

        public ActionResult Delete(Guid id)
        {
            var lancamento = _LancamentoApp.GetById(id);
            return View(lancamento);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var lancamento = _LancamentoApp.GetByIdReadOnly(id);
            _LancamentoApp.RemoveTransferencia(lancamento);

            return RedirectToAction("Index", "Lancamentos", new { contaIdFiltro = lancamento.ContaId, MesAnoCompetencia = lancamento.DataVencimento });
        }

        private void SetSelectLists(Guid? contaIdOrigem = null)
        {
            if (contaIdOrigem == null)
                contaIdOrigem = Guid.Empty;

            ViewBag.Contas = new SelectList(_ContaApp.GetAll().Where(c => c.ContaId != contaIdOrigem) , "ContaId", "Descricao");
            ViewBag.Categorias = new SelectList(_CategoriaApp.GetAll(), "CategoriaId", "Descricao");
        }

    }
}