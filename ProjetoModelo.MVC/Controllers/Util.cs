using Moneta.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moneta.MVC.Controllers
{
    public class Util
    {
        public static void TratarLancamentoValorHtml5Number(LancamentoViewModel lancamento, ModelStateDictionary ModelState)
        {
            if (lancamento.Valor == 0 && ModelState["Valor"].Value.AttemptedValue != "")
            {
                lancamento.Valor = Decimal.Parse(ModelState["Valor"].Value.AttemptedValue.Replace('.', ','));
                ModelState["Valor"].Errors.Clear();
            }
        }

        public static void TratarLancamentoValorHtml5Number(TransferenciaViewModel transferecia, ModelStateDictionary ModelState)
        {
            if (transferecia.LancamentoOrigem.Valor == 0 && ModelState["LancamentoOrigem.Valor"].Value.AttemptedValue != "")
            {
                transferecia.LancamentoOrigem.Valor = Decimal.Parse(ModelState["LancamentoOrigem.Valor"].Value.AttemptedValue.Replace('.', ','));
                ModelState["LancamentoOrigem.Valor"].Errors.Clear();
            }
        }
    }
}