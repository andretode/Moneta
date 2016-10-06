using System;
using System.Linq;
using System.Web.Mvc;
using Moneta.Application.Interfaces;
using Moneta.Application.ViewModels;

namespace Moneta.MVC.Controllers
{
    public class ContasController : Controller
    {
        private readonly IContaAppService _contaApp;

        public ContasController(
            IContaAppService contaApp)
        {
            _contaApp = contaApp;
        }

        // GET: Conta
        public ViewResult Index(string pesquisa, int page = 0)
        {
            var contaViewModel = _contaApp.ObterContasGrid(page * 5, pesquisa);
            ViewBag.PaginaAtual = page;
            ViewBag.Pesquisa = pesquisa;
            ViewBag.TotalRegistros = _contaApp.ObterTotalRegistros(pesquisa);


            return View(contaViewModel);
        }


        // GET: Conta/Details/5
        public ActionResult Details(Guid id)
        {
            var contaViewModel = _contaApp.GetById(id);

            return View(contaViewModel);
        }

        // GET: Conta/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Conta/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ContaViewModel contaEndereco)
        {
            if (ModelState.IsValid)
            {
                var result = _contaApp.Add(contaEndereco);

                if (!result.IsValid)
                {
                    foreach (var validationAppError in result.Erros)
                    {
                        ModelState.AddModelError(string.Empty, validationAppError.Message);
                    }
                    return View(contaEndereco);
                }

                return RedirectToAction("Index");
            }
     
            return View(contaEndereco);
        }

        // GET: Conta/Edit/5
        public ActionResult Edit(Guid id)
        {
            var contaViewModel = _contaApp.GetById(id);

            return View(contaViewModel);
        }

        // POST: Conta/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ContaViewModel contaViewModel)
        {
            if (ModelState.IsValid)
            {
                _contaApp.Update(contaViewModel);

                return RedirectToAction("Index");
            }

            return View(contaViewModel);
        }

        // GET: Conta/Delete/5
        public ActionResult Delete(Guid id)
        {
            var contaViewModel = _contaApp.GetById(id);

            return View(contaViewModel);
        }

        // POST: Conta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var conta = _contaApp.GetById(id);
            _contaApp.Remove(conta);

            return RedirectToAction("Index");
        }
    }
}
