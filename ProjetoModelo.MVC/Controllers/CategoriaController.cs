using System;
using System.Linq;
using System.Web.Mvc;
using Moneta.Application.Interfaces;
using Moneta.Application.ViewModels;

namespace Moneta.MVC.Controllers
{
    [Authorize(Roles = "UsuarioPlanoBasico")]
    public class CategoriasController : Controller
    {
        private readonly ICategoriaAppService _categoriaApp;

        public CategoriasController(
            ICategoriaAppService categoriaApp)
        {
            _categoriaApp = categoriaApp;
        }

        // GET: Categoria
        public ViewResult Index()
        {
            var categoriaViewModel = _categoriaApp.GetAll().OrderBy(c => c.Descricao);
            return View(categoriaViewModel);
        }


        // GET: Categoria/Details/5
        public ActionResult Details(Guid id)
        {
            var categoriaViewModel = _categoriaApp.GetById(id);

            return View(categoriaViewModel);
        }

        // GET: Categoria/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categoria/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoriaViewModel categoriaEndereco)
        {
            if (ModelState.IsValid)
            {
                var result = _categoriaApp.Add(categoriaEndereco);

                if (!result.IsValid)
                {
                    foreach (var validationAppError in result.Erros)
                    {
                        ModelState.AddModelError(string.Empty, validationAppError.Message);
                    }
                    return View(categoriaEndereco);
                }

                return RedirectToAction("Index");
            }
     
            return View(categoriaEndereco);
        }

        // GET: Categoria/Edit/5
        public ActionResult Edit(Guid id)
        {
            var categoriaViewModel = _categoriaApp.GetById(id);

            return View(categoriaViewModel);
        }

        // POST: Categoria/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CategoriaViewModel categoriaViewModel)
        {
            if (ModelState.IsValid)
            {
                _categoriaApp.Update(categoriaViewModel);

                return RedirectToAction("Index");
            }

            return View(categoriaViewModel);
        }

        // GET: Categoria/Delete/5
        public ActionResult Delete(Guid id)
        {
            var categoriaViewModel = _categoriaApp.GetById(id);

            return View(categoriaViewModel);
        }

        // POST: Categoria/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var categoria = _categoriaApp.GetAllReadOnly().Where(c => c.CategoriaId == id).First(); //_categoriaApp.GetById(id);
            _categoriaApp.Remove(categoria);

            return RedirectToAction("Index");
        }
    }
}
