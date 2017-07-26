using Moneta.Application.Interfaces;
using Moneta.Application.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Moneta.Services.WebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LancamentoController : ApiController
    {
        private readonly ILancamentoAppService _lancamentoApp;

        public LancamentoController(ILancamentoAppService lancamentoApp)
        {
            _lancamentoApp = lancamentoApp;
        }

        [HttpPost]
        public void Remove(LancamentoViewModel lancamento)
        {
            _lancamentoApp.Remove(lancamento);
        }

        [HttpPost]
        public void Post([FromBody] IEnumerable<LancamentoViewModel> lancamentos)
        {
            if (ModelState.IsValid)
            {
                foreach (var l in lancamentos)
                    _lancamentoApp.Add(l);
            }
            else
            {
                throw new Exception("Dados invalidos");
            }
        }

        public LancamentosDoMesViewModel Get(string lancamentosJson)
        {
            var lancamentos = JsonConvert.DeserializeObject<LancamentosDoMesViewModel>(lancamentosJson);
            if (lancamentos.MesAnoCompetencia == DateTime.MinValue)
                lancamentos.MesAnoCompetencia = DateTime.Now;

            var x = _lancamentoApp.GetLancamentosDoMes(lancamentos);
            return x;
        }
    }
}