﻿using System;
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
            if (lancamentos.MesAnoCompetencia == DateTime.MinValue)
                lancamentos.MesAnoCompetencia = DateTime.Now;

            DateTime mesAnoCompetenciaTemp = lancamentos.MesAnoCompetencia;
            Guid contaIdFiltroTemp = lancamentos.ContaIdFiltro;

            lancamentos = _LancamentoApp.GetLancamentosDoMes(lancamentos);
            lancamentos.MesAnoCompetencia = mesAnoCompetenciaTemp;
            lancamentos.ContaIdFiltro = contaIdFiltroTemp;

            SetSelectLists();

            return View(lancamentos);
        }

        public ActionResult GraficoSaldoDoMes()
        {
            var grafico = new GraficoSaldoViewModel();
            var lancamentosDoMes = new LancamentosDoMesViewModel();
            lancamentosDoMes.ContaIdFiltro = Guid.Parse("d6c0b606-5a1b-4898-b834-c2d2bf4f8c34");
            lancamentosDoMes.MesAnoCompetencia = DateTime.Now;
            var listaSaldoDoMesPorDia = _LancamentoApp.GetSaldoDoMesPorDia(lancamentosDoMes);
            grafico.SaldoPorDia = new List<SaldoNoDiaViewModel>();
            foreach (var saldoPorDia in listaSaldoDoMesPorDia)
            {
                grafico.SaldoPorDia.Add(new SaldoNoDiaViewModel() { Dia = saldoPorDia.Item1, Saldo = saldoPorDia.Item2 });
            }

            return View(grafico);
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
                if (lancamento.Fake)
                    _LancamentoApp.Add(lancamento);
                else
                    _LancamentoApp.Update(lancamento);


                return RedirectToAction("Index", new { contaIdFiltro = lancamento.ContaId, MesAnoCompetencia = lancamento.DataVencimento });
            }

            return View(lancamento);
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
                lancamento.Ativo = false;
                if (lancamento.Fake)
                    _LancamentoApp.Add(lancamento);
                else
                {
                    //se não for parcelado, remove do BD, senão mantém para controlar os fakes que tem que ser mostrados
                    if (lancamento.IdDaParcelaNaSerie == null)
                        _LancamentoApp.Remove(lancamento);
                    else
                        _LancamentoApp.Update(lancamento);
                }

                return RedirectToAction("Index", new { contaIdFiltro = lancamento.ContaId, MesAnoCompetencia = lancamento.DataVencimento });
            }
            return View(lancamento);
        }


        #region Metodos Privados
        private void RecomporLancamento(LancamentoViewModel lancamento)
        {
            lancamento.Conta = _ContaApp.GetById(lancamento.ContaId);
            lancamento.Categoria = _CategoriaApp.GetById(lancamento.CategoriaId);
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
