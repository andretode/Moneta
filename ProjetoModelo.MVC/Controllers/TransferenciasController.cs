using Moneta.Application.Interfaces;
using Moneta.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moneta.MVC.Controllers
{
    [Authorize(Roles = "UsuarioPlanoBasico")]
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

        public ActionResult Create(LancamentosDoMesViewModel lancamentos)
        {
            if (ModelState.IsValid)
            {
                var cookieContaId = Util.GetCookieContaId(Request, Response);
                lancamentos.ContaIdFiltro = Guid.Parse(cookieContaId.Value.ToString());

                var novaTransferencia = new TransferenciaViewModel(lancamentos.MesAnoCompetencia);
                novaTransferencia.LancamentoPai.Conta = _ContaApp.GetById((Guid)lancamentos.ContaIdFiltro);
                novaTransferencia.LancamentoPai.CategoriaId = _CategoriaApp.GetAll().Where(c => c.Descricao == CategoriaViewModel.Nenhum).FirstOrDefault().CategoriaId;
                SetSelectLists((Guid)lancamentos.ContaIdFiltro);
                return View(novaTransferencia);
            }
            return RedirectToAction("Index", new { lancamentos });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TransferenciaViewModel transferencia)
        {
            Util.TratarLancamentoValorHtml5Number(transferencia, ModelState);

            if (ModelState.IsValid)
            {
                transferencia.ContaIdOrigem = transferencia.LancamentoPai.Conta.ContaId;
                if (transferencia.LancamentoPai.Valor > 0)
                    transferencia.LancamentoPai.Valor *= -1;

                var result = _LancamentoApp.AddTransferencia(transferencia);

                if (!result.IsValid)
                {
                    foreach (var validationAppError in result.Erros)
                    {
                        ModelState.AddModelError(string.Empty, validationAppError.Message);
                    }
                    return View(transferencia);
                }

                return RedirectToAction("Index", "Lancamentos", new
                {
                    MesAnoCompetencia = transferencia.LancamentoPai.DataVencimento
                });
            }

            SetSelectLists(transferencia.LancamentoPai.ContaId);
            transferencia.LancamentoPai.Conta = _ContaApp.GetById(transferencia.LancamentoPai.ContaId);
            return View(transferencia);
        }

        public ActionResult CreateFromExtrato(LancamentoViewModel lancamento)
        {
            SetSelectLists();
            lancamento.Conta = _ContaApp.GetById(lancamento.ContaId);
            lancamento.CategoriaId = _CategoriaApp.GetAll()
                .Where(c => c.Descricao == CategoriaViewModel.Nenhum)
                .FirstOrDefault().CategoriaId;
            var transferencia = new TransferenciaViewModel();
            transferencia.LancamentoPai = lancamento;

            if (lancamento.Valor < 0)
            {
                transferencia.ContaIdOrigem = lancamento.ContaId;
                transferencia.ConciliarExtratoCom = ContaEnum.ORIGEM;
            }
            else
            {
                transferencia.ContaIdDestino = lancamento.ContaId;
                transferencia.ConciliarExtratoCom = ContaEnum.DESTINO;
            }

            return View(transferencia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFromExtrato(TransferenciaViewModel transferencia)
        {
            Util.TratarLancamentoValorHtml5Number(transferencia, ModelState);

            if (ModelState.IsValid)
            {
                var result = _LancamentoApp.AddTransferencia(transferencia);

                if (!result.IsValid)
                {
                    foreach (var validationAppError in result.Erros)
                    {
                        ModelState.AddModelError(string.Empty, validationAppError.Message);
                    }
                    return View(transferencia);
                }

                return RedirectToAction("Index", "ExtratosBancarios", new
                {
                    MesAnoCompetencia = transferencia.LancamentoPai.DataVencimento
                });
            }

            SetSelectLists(transferencia.LancamentoPai.ContaId);
            transferencia.LancamentoPai.Conta = _ContaApp.GetById(transferencia.LancamentoPai.ContaId);
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
            Util.TratarLancamentoValorHtml5Number(lancamento, ModelState);

            if (ModelState.IsValid)
            {
                _LancamentoApp.Update(lancamento);
                
                return RedirectToAction("Index", "Lancamentos", new { 
                    MesAnoCompetencia = lancamento.DataVencimento 
                });
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

            return RedirectToAction("Index", "Lancamentos", new { 
                MesAnoCompetencia = lancamento.DataVencimento
            });
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