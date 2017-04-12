﻿using System;
using System.Linq;
using System.Web.Mvc;
using Moneta.Application.Interfaces;
using Moneta.Application.ViewModels;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using Moneta.Infra.CrossCutting.Enums;

namespace Moneta.MVC.Controllers
{
    public class ExtratosBancariosController : Controller
    {
        private readonly IExtratoBancarioAppService _ExtratoBancarioApp;
        private readonly IContaAppService _ContaApp;

        public ExtratosBancariosController(
            IExtratoBancarioAppService extratoBancarioApp,
            IContaAppService contaApp)
        {
            _ExtratoBancarioApp = extratoBancarioApp;
            _ContaApp = contaApp;
        }

        // GET: Categoria
        public ViewResult Index(string pesquisa, int page = 0)
        {
            var extratoBancarioViewModel = _ExtratoBancarioApp.GetAll().OrderBy(c => c.DataCompensacao);
            ViewBag.Pesquisa = pesquisa;

            return View(extratoBancarioViewModel);
        }


        //// GET: Categoria/Details/5
        //public ActionResult Details(Guid id)
        //{
        //    var categoriaViewModel = _categoriaApp.GetById(id);

        //    return View(categoriaViewModel);
        //}

        // GET: Categoria/Create
        public ActionResult Create()
        {
            return View();
        }


        public ActionResult ImportarOfx()
        {
            SetSelectLists();
            return View();
        }

        public ActionResult CriarLancamento(Guid id)
        {
            var eb =_ExtratoBancarioApp.GetById(id);

            var lancamento = new LancamentoViewModel()
            {
                Conta = eb.Conta,
                ContaId = eb.ContaId,
                DataVencimento = eb.DataCompensacao,
                Descricao = eb.Descricao,
                ExtratoBancarioId = eb.ExtratoBancarioId,
                Fake = false,
                Ativo = true,
                NumeroDocumento = eb.NumeroDocumento,
                Pago = true,
                Valor = eb.Valor,
                TipoDeTransacao = (eb.Valor <= 0 ? TipoTransacaoEnum.Despesa : TipoTransacaoEnum.Receita)
            };

            return RedirectToAction("CreateFromExtrato", "Lancamentos", lancamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportarOfx(string arquivoOfx)
        {
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
                _ExtratoBancarioApp.ImportarOfx(caminhoOfx, contaId);
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

            return RedirectToAction("Index");
        }

        public ActionResult Delete(Guid id)
        {
            var extratoBancario = _ExtratoBancarioApp.GetAllReadOnly().Where(c => c.ExtratoBancarioId == id).First();
            _ExtratoBancarioApp.Remove(extratoBancario);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoverSelecionados(IEnumerable<ExtratoBancarioViewModel> extratos)
        {
            _ExtratoBancarioApp.RemoveAll(extratos.Where(ex => ex.Selecionado));
            return RedirectToAction("Index");
        }

        private void SetSelectLists()
        {
            ViewBag.Contas = new SelectList(_ContaApp.GetAll(), "ContaId", "Descricao");
        }
    }
}
