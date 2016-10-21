using Moneta.Application.Interfaces;
using Moneta.Application.ViewModels;
using System;
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
            return View();
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult SignOut()
        {
            ViewBag.Message = "Sair";

            return View();
        }
    }
}