﻿using Moneta.Application.Interfaces;
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
    public class GrupoLancamentosController : Controller
    {
        private readonly IGrupoLancamentoAppService _grupoLancamentoApp;
        private readonly IContaAppService _contaApp;
        private readonly ILancamentoAppService _lancamentoApp;

        public GrupoLancamentosController(
            IGrupoLancamentoAppService grupoLancamentoApp,
            IContaAppService contaApp,
            ILancamentoAppService lancamentoApp)
        {
            _grupoLancamentoApp = grupoLancamentoApp;
            _contaApp = contaApp;
            _lancamentoApp = lancamentoApp;
        }

        // GET: GrupoLancamentos
        public ActionResult Index()
        {
            var gruposLancamento = _grupoLancamentoApp.GetAll().OrderBy(c => c.Descricao);
            return View(gruposLancamento);
        }

        // GET: GrupoLancamentos/Details/5
        public ActionResult Details(Guid id)
        {
            var grupoLancamento = _grupoLancamentoApp.GetById(id);
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

        public ActionResult ImportarOfx()
        {
            SetSelectLists();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportarOfx(string arquivoOfx)
        {
            int quantidadeImportada = 0;
            string caminhoOfx = "";
            Guid contaId = Guid.Parse(Request.Form.Get("contaIdFiltro"));

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
                return View();
            }

            try
            {
                quantidadeImportada = 0; //_ExtratoBancarioApp.ImportarOfx(caminhoOfx, contaId);
            }
            catch (FormatException fx)
            {
                ModelState.AddModelError(string.Empty, "O arquivo enviado tem um formato OFX desconhecido. Detalhes técnicos do erro: " + fx.Message);
                SetSelectLists();
                return View();
            }
            catch (XmlException xe)
            {
                ModelState.AddModelError(string.Empty, "O arquivo enviado tem um formato OFX fora do padrão. Detalhes técnicos do erro: " + xe.Message);
                SetSelectLists();
                return View();
            }

            return RedirectToAction("Index", new { quant = quantidadeImportada });
        }

        private void SetSelectLists()
        {
            ViewBag.Contas = new SelectList(_contaApp.GetAll(), "ContaId", "Descricao");
        }
    }
}
