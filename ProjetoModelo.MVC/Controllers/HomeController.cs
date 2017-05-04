using Moneta.Application.Interfaces;
using Moneta.Application.ViewModels;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Moneta.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILancamentoAppService _LancamentoApp;
        private readonly IContaAppService _ContaApp;
        public HomeController(ILancamentoAppService LancamentoApp,
            IContaAppService ContaApp)
        {
            _LancamentoApp = LancamentoApp;
            _ContaApp = ContaApp;
        }

        public ActionResult Index(GraficosViewModel graficosViewModel)
        {
            graficosViewModel.GraficoSaldoDoMes = GetDadosSaldoDoMes(graficosViewModel.ContaIdFiltro);
            graficosViewModel.GraficoSaldoPorCategoria = GetDadosSaldoPorCategoria(graficosViewModel.ContaIdFiltro, graficosViewModel.MesAnoCompetencia);

            SetSelectLists();
            return View(graficosViewModel);
        }

        public ViewResult AlterarMes(DateTime mesAnoCompetencia, Guid contaIdFiltro, int addMonths)
        {
            mesAnoCompetencia = mesAnoCompetencia.AddMonths(addMonths);

            var graficosViewModel = new GraficosViewModel();
            graficosViewModel.MesAnoCompetencia = mesAnoCompetencia;
            graficosViewModel.ContaIdFiltro = contaIdFiltro;
            graficosViewModel.GraficoSaldoDoMes = GetDadosSaldoDoMes(graficosViewModel.ContaIdFiltro);
            graficosViewModel.GraficoSaldoPorCategoria = GetDadosSaldoPorCategoria(graficosViewModel.ContaIdFiltro, mesAnoCompetencia);

            SetSelectLists();
            return View("Index", graficosViewModel);
        }

        private GraficoSaldoDoMesViewModel GetDadosSaldoDoMes(Guid? ContaIdFiltro)
        {
            ContaIdFiltro = (ContaIdFiltro == null ? Guid.Empty : (Guid)ContaIdFiltro);

            var lancamentosDoMes = new LancamentosDoMesViewModel();
            lancamentosDoMes.ContaIdFiltro = (Guid)ContaIdFiltro;
            lancamentosDoMes.MesAnoCompetencia = DateTime.Now;

            return new GraficoSaldoDoMesViewModel(_LancamentoApp.GetSaldoDoMesPorDia(lancamentosDoMes, false));
        }

        private GraficoSaldoPorCategoriaViewModel GetDadosSaldoPorCategoria(Guid ContaIdFiltro, DateTime mesAnoCompetencia)
        {
            return _LancamentoApp.GetDespesasPorCategoria(ContaIdFiltro, mesAnoCompetencia);
        }

        public ActionResult SignOut()
        {
            ViewBag.Message = "Sair";

            return View();
        }

        private void SetSelectLists()
        {
            ViewBag.Contas = new SelectList(_ContaApp.GetAll(), "ContaId", "Descricao");
        }
    }
}