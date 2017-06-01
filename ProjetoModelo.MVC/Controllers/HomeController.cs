using Moneta.Application.Interfaces;
using Moneta.Application.ViewModels;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;
using System.Net;

namespace Moneta.MVC.Controllers
{
    [Authorize(Roles = "UsuarioPlanoBasico")]
    public class HomeController : Controller
    {
        private readonly ILancamentoAppService _LancamentoApp;
        private readonly IContaAppService _ContaApp;
        private readonly ICategoriaAppService _CategoriaApp;

        public HomeController(ILancamentoAppService LancamentoApp,
            IContaAppService ContaApp,
            ICategoriaAppService CategoriaApp)
        {
            _LancamentoApp = LancamentoApp;
            _ContaApp = ContaApp;
            _CategoriaApp = CategoriaApp;
        }

        public JsonResult TrocarConta(string ContaIdFiltro)
        {
            Util.SetValorContaIdCookie(ContaIdFiltro, Response);

            return new JsonResult()
            {
                Data = new { status = "Ok" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        public ActionResult Index(GraficosViewModel graficosViewModel)
        {
            var cookieContaId = Util.GetCookieContaId(Request, Response);

            graficosViewModel.ContaIdFiltro = Guid.Parse(cookieContaId.Value.ToString());
            graficosViewModel.GraficoSaldoDoMes = GetDadosSaldoDoMes(graficosViewModel.ContaIdFiltro);
            graficosViewModel.GraficoSaldoPorCategoria = GetDadosSaldoPorCategoria(graficosViewModel.ContaIdFiltro, graficosViewModel.MesAnoCompetencia);
            graficosViewModel.GraficoOrcadoVsRealizado = GetDadosSaldoPorCategoria(graficosViewModel.ContaIdFiltro, graficosViewModel.MesAnoCompetencia, true);

            SetSelectLists();
            return View(graficosViewModel);
        }
        
        [HttpGet]
        public JsonResult GetDadosDespesasPorCategoria(GraficosViewModel graficosViewModel)
        {
            JsonResult jsonResult;

            try
            {
                var graficoSaldoPorCategoria = GetDadosSaldoPorCategoria(graficosViewModel.ContaIdFiltro, graficosViewModel.MesAnoCompetencia, graficosViewModel.SomentePagos);

                jsonResult = new JsonResult()
                {
                    Data = new
                    {
                        status = "Ok",
                        arrayDeCategorias = graficoSaldoPorCategoria.ArrayDeCategorias,
                        arrayDeCores = graficoSaldoPorCategoria.ArrayDeCores,
                        arrayDeSaldos = graficoSaldoPorCategoria.ArrayDeSaldosRealizados
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

            } catch(Exception ex)
            {
                jsonResult = new JsonResult()
                {
                    Data = new { status = "Nok", erro = ex.Message },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            return jsonResult;
        }

        public ViewResult AlterarMes(DateTime mesAnoCompetencia, Guid contaIdFiltro, int addMonths)
        {
            mesAnoCompetencia = mesAnoCompetencia.AddMonths(addMonths);

            var graficosViewModel = new GraficosViewModel();
            graficosViewModel.MesAnoCompetencia = mesAnoCompetencia;
            graficosViewModel.ContaIdFiltro = contaIdFiltro;
            graficosViewModel.GraficoSaldoDoMes = GetDadosSaldoDoMes(graficosViewModel.ContaIdFiltro);
            graficosViewModel.GraficoSaldoPorCategoria = GetDadosSaldoPorCategoria(graficosViewModel.ContaIdFiltro, mesAnoCompetencia);
            graficosViewModel.GraficoOrcadoVsRealizado = GetDadosSaldoPorCategoria(graficosViewModel.ContaIdFiltro, graficosViewModel.MesAnoCompetencia, true);

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

        private GraficoSaldoPorCategoriaViewModel GetDadosSaldoPorCategoria(Guid ContaIdFiltro, DateTime mesAnoCompetencia, bool SomentePagos = false)
        {
            return _LancamentoApp.GetDespesasPorCategoria(ContaIdFiltro, mesAnoCompetencia, SomentePagos);
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