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
        public HomeController(ILancamentoAppService LancamentoApp)
        {
            _LancamentoApp = LancamentoApp;
        }

        public ActionResult Index()
        {
            var graficosViewModel = new GraficosViewModel();
            graficosViewModel.GraficoSaldoDoMes = GetDadosSaldoDoMes(Guid.Empty);
            graficosViewModel.GraficoSaldoPorCategoria = GetDadosSaldoPorCategoria();

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

        private GraficoSaldoPorCategoriaViewModel GetDadosSaldoPorCategoria()
        {
            return new GraficoSaldoPorCategoriaViewModel(_LancamentoApp.GetSaldoPorCategoria());
        }

        public ActionResult SignOut()
        {
            ViewBag.Message = "Sair";

            return View();
        }
    }
}