using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Moneta.Application.ViewModels
{
    public class LancamentosViewModel
    {
        [DisplayName("Filtro Conta")]
        [Required(ErrorMessage = "Selecione uma conta")]
        public Guid ContaIdFiltro { get; set; }

        [DisplayName("Transação")]
        public TipoTransacao NovaTransacao { get; set; }

        [DisplayName("Saldo do Mês")]
        [DataType(DataType.Currency)]
        public decimal SaldoDoMes { get; set; }

        public virtual IEnumerable<LancamentoViewModel> Lancamentos { get; set; }
    }

    public enum TipoTransacao { Receita, Despesa, Transferencia }
}