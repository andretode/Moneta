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
            if (transferecia.LancamentoPai.Valor == 0 && ModelState["LancamentoPai.Valor"].Value.AttemptedValue != "")
            {
                transferecia.LancamentoPai.Valor = Decimal.Parse(ModelState["LancamentoPai.Valor"].Value.AttemptedValue.Replace('.', ','));
                ModelState["LancamentoPai.Valor"].Errors.Clear();
            }
        }
    }
}