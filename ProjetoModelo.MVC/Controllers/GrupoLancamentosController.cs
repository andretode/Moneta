using Moneta.Application.Interfaces;
using Moneta.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace Moneta.MVC.Controllers
{
    [Authorize(Roles = "UsuarioPlanoBasico")]
    public class GrupoLancamentosController : Controller
    {
        private readonly IGrupoLancamentoAppService _grupoLancamentoApp;
        private readonly IContaAppService _contaApp;
        private readonly ILancamentoAppService _lancamentoApp;
        private readonly ICategoriaAppService _categoriaApp;

        public GrupoLancamentosController(
            IGrupoLancamentoAppService grupoLancamentoApp,
            IContaAppService contaApp,
            ILancamentoAppService lancamentoApp,
            ICategoriaAppService categoriaApp)
        {
            _grupoLancamentoApp = grupoLancamentoApp;
            _contaApp = contaApp;
            _lancamentoApp = lancamentoApp;
            _categoriaApp = categoriaApp;
        }

        // GET: GrupoLancamentos
        public ActionResult Index()
        {
            var gruposLancamento = _grupoLancamentoApp.GetAllExcetoGruposFilhos();
            return View(gruposLancamento);
        }

        [HttpGet]
        public ActionResult PesquisarGrupos(string descricaoGrupoPesquisa, Guid grupoLancamentoIdPai)
        {
            var grupoPesquisaViewModel = new GrupoPesquisaViewModel { DescricaoGrupoPesquisa = descricaoGrupoPesquisa };

            if(descricaoGrupoPesquisa.Length > 2)
            {
                grupoPesquisaViewModel.Grupos = FilterGrupoByDescricao(descricaoGrupoPesquisa, grupoLancamentoIdPai);
            }

            return PartialView("_ListagemGruposPesquisados", grupoPesquisaViewModel);
        }

        [HttpPost]
        public JsonResult IncluirGrupoLancamento(Guid grupoLancamentoIdPai, Guid grupoLancamentoIdFilho)
        {
            var jsonResult = new JsonResult()
            {
                Data = new { status = "Ok" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            try
            {
                var grupoLancamentoFilho = _grupoLancamentoApp.GetByIdReadOnly(grupoLancamentoIdFilho);
                grupoLancamentoFilho.GrupoLancamentoIdPai = grupoLancamentoIdPai;
                _grupoLancamentoApp.Update(grupoLancamentoFilho);
            }
            catch (Exception)
            {
                jsonResult.Data = new { status = "Nok" };
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return jsonResult;
        }

        private IEnumerable<GrupoLancamentoViewModel> FilterGrupoByDescricao(string descricaoGrupoPesquisa, Guid grupoLancamentoIdPai)
        {
            var filtros = descricaoGrupoPesquisa.Split(' ');
            var _todosOsGrupos = _grupoLancamentoApp.GetAllExcetoGruposFilhos().Where(g => g.GrupoLancamentoId != grupoLancamentoIdPai);
            IEnumerable<GrupoLancamentoViewModel> gruposFiltrados = null;

            switch (filtros.Count())
            {
                case 1:
                    gruposFiltrados = from grupos in _todosOsGrupos
                                         where grupos.Descricao.ToLowerInvariant().Contains(filtros[0].ToLowerInvariant())
                                         select grupos;
                    break;
                case 2:
                    gruposFiltrados = from grupos in _todosOsGrupos
                                         where grupos.Descricao.ToLowerInvariant().Contains(filtros[0].ToLowerInvariant())
                                         && grupos.Descricao.ToLowerInvariant().Contains(filtros[1].ToLowerInvariant())
                                         select grupos;
                    break;
                case 3:
                    gruposFiltrados = from grupos in _todosOsGrupos
                                         where grupos.Descricao.ToLowerInvariant().Contains(filtros[0].ToLowerInvariant())
                                         && grupos.Descricao.ToLowerInvariant().Contains(filtros[1].ToLowerInvariant())
                                         && grupos.Descricao.ToLowerInvariant().Contains(filtros[2].ToLowerInvariant())
                                         select grupos;
                    break;
                case 4:
                    gruposFiltrados = from grupos in _todosOsGrupos
                                      where grupos.Descricao.ToLowerInvariant().Contains(filtros[0].ToLowerInvariant())
                                        && grupos.Descricao.ToLowerInvariant().Contains(filtros[1].ToLowerInvariant())
                                        && grupos.Descricao.ToLowerInvariant().Contains(filtros[2].ToLowerInvariant())
                                        && grupos.Descricao.ToLowerInvariant().Contains(filtros[3].ToLowerInvariant())
                                        select grupos;
                    break;
                default:
                    gruposFiltrados = from grupos in _todosOsGrupos
                                      where grupos.Descricao.ToLowerInvariant().Contains(descricaoGrupoPesquisa.ToLowerInvariant())
                                      select grupos;
                    break;
            }



            return gruposFiltrados.ToList();
        }

        // GET: GrupoLancamentos/Details/5
        public ActionResult Details(Guid id, int? quant)
        {
            ViewBag.quantidadeImportada = quant;
            var grupoLancamento = _grupoLancamentoApp.GetById(id);
            SetSelectLists();
            return View(grupoLancamento);
        }

        // GET: GrupoLancamentos/Create
        public ActionResult Create()
        {
            SetSelectLists();
            return View();
        }

        // POST: GrupoLancamentos/Create
        [HttpPost]
        public ActionResult Create(GrupoLancamentoViewModel grupoLancamento)
        {
            if (ModelState.IsValid)
            {
                var result = _grupoLancamentoApp.Add(grupoLancamento);

                if (!result.IsValid)
                {
                    foreach (var validationAppError in result.Erros)
                    {
                        ModelState.AddModelError(string.Empty, validationAppError.Message);
                    }
                    return View(grupoLancamento);
                }

                return RedirectToAction("Index");
            }

            SetSelectLists();
            return View(grupoLancamento);
        }

        public ActionResult Dividir(GrupoLancamentoViewModel grupoLancamento)
        {
            if (ModelState.IsValid)
            {
                _lancamentoApp.Remove(_lancamentoApp.GetByIdReadOnly((Guid)grupoLancamento.LancamentoIdDividido));
                var result = _grupoLancamentoApp.Add(grupoLancamento);
                return RedirectToAction("Details", new { id = grupoLancamento.GrupoLancamentoId });
            }

            SetSelectLists();
            return View("Create", grupoLancamento);
        }

        // GET: GrupoLancamentos/Edit/5
        public ActionResult Edit(Guid id)
        {
            var grupoLancamento = _grupoLancamentoApp.GetById(id);
            SetSelectLists();
            return View(grupoLancamento);
        }

        // POST: GrupoLancamentos/Edit/5
        [HttpPost]
        public ActionResult Edit(GrupoLancamentoViewModel grupoLancamento)
        {
            if (ModelState.IsValid)
            {
                _grupoLancamentoApp.Update(grupoLancamento);

                return RedirectToAction("Index");
            }

            SetSelectLists();
            return View(grupoLancamento);
        }

        // GET: GrupoLancamentos/Delete/5
        public ActionResult Delete(Guid id)
        {
            var grupoLancamentoViewModel = _grupoLancamentoApp.GetAllReadOnly().Where(c => c.GrupoLancamentoId == id).First();
            _grupoLancamentoApp.Remove(grupoLancamentoViewModel);
            return RedirectToAction("Index");
        }

        public ActionResult AdicionarTransferencia(Guid grupoLancamentoId)
        {
            var transferenciaGrupoLancamento = new TransferenciaGrupoLancamentoViewModel(grupoLancamentoId);
            return View(transferenciaGrupoLancamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdicionarDespesa(LancamentoViewModel lancamento)
        {
            if (ModelState.IsValid)
            {
                _lancamentoApp.Add(lancamento);
                return RedirectToAction("Details", new { id = lancamento.GrupoLancamentoId });
            }

            return View("Details", lancamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdicionarTransferencia(TransferenciaGrupoLancamentoViewModel transferenciaGrupoLancamento)
        {
            if (ModelState.IsValid)
            {
                var lancamento = _lancamentoApp.GetByIdReadOnly(Guid.Parse(transferenciaGrupoLancamento.TransferenciaId));
                lancamento.GrupoLancamentoId = transferenciaGrupoLancamento.GrupoLancamentoId;
                _lancamentoApp.Update(lancamento);
                return RedirectToAction("Index");
            }

            return View(transferenciaGrupoLancamento);
        }

        public ActionResult ImportarOfx(GrupoLancamentoViewModel grupo)
        {
            SetSelectLists();
            return View(grupo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportarOfx(string arquivoOfx, GrupoLancamentoViewModel grupo)
        {
            int quantidadeImportada = 0;
            string caminhoOfx = "";

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    caminhoOfx = Path.Combine(Server.MapPath("~/"), fileName);
                    file.SaveAs(caminhoOfx);
                }
            }

            if (caminhoOfx == "")
            {
                ModelState.AddModelError(string.Empty, "Ocorreu um erro ao salvar o arquivo OFX enviado.");
                SetSelectLists();
                return View(grupo);
            }

            try
            {
                quantidadeImportada = _lancamentoApp.ImportarOfxParaGrupoDeLancamento(caminhoOfx, grupo.ContaId, grupo.DataVencimento, grupo.GrupoLancamentoId);
            }
            catch (FormatException fx)
            {
                ModelState.AddModelError(string.Empty, "O arquivo enviado tem um formato OFX desconhecido. Detalhes técnicos do erro: " + fx.Message);
                SetSelectLists();
                return View(grupo);
            }
            catch (XmlException xe)
            {
                ModelState.AddModelError(string.Empty, "O arquivo enviado tem um formato OFX fora do padrão. Detalhes técnicos do erro: " + xe.Message);
                SetSelectLists();
                return View(grupo);
            }

            return RedirectToAction("Details", new { id = grupo.GrupoLancamentoId, quant = quantidadeImportada });
        }

        private void SetSelectLists()
        {
            ViewBag.Contas = new SelectList(_contaApp.GetAll(), "ContaId", "Descricao");
            var categorias = _categoriaApp.GetAll();
            ViewBag.Categorias = new SelectList(categorias, "CategoriaId", "Descricao");
            ViewBag.CoresCategoria = categorias.Select(c => c.Cor).ToList();
        }
    }
}
