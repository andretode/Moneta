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

        public ActionResult Index(Guid? ContaIdFiltro)
        {
            var graficosViewModel = new GraficosViewModel();
            graficosViewModel.GraficoSaldoDoMes = GetDadosSaldoDoMes(ContaIdFiltro);
            graficosViewModel.GraficoSaldoPorCategoria = GetDadosSaldoPorCategoria(ContaIdFiltro);

            SetSelectLists();
            return View(graficosViewModel);
        }

        private GraficoSaldoDoMesViewModel GetDadosSaldoDoMes(Guid? ContaIdFiltro)
        {
            ContaIdFiltro = (ContaIdFiltro == null ? Guid.Empty : (Guid)ContaIdFiltro);

            var lancamentosDoMes = new LancamentosDoMesViewModel();
            lancamentosDoMes.ContaIdFiltro = (Guid)ContaIdFiltro;
            lancamentosDoMes.MesAnoCompetencia = DateTime.Now;

            return new GraficoSaldoDoMesViewModel(_LancamentoApp.GetSaldoDoMesPorDia(lancamentosDoMes, false));
        }

        private GraficoSaldoPorCategoriaViewModel GetDadosSaldoPorCategoria(Guid? ContaIdFiltro)
        {
            return _LancamentoApp.GetDespesasPorCategoria(ContaIdFiltro);
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