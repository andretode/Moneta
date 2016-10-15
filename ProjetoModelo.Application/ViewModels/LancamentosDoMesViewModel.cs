using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Moneta.Application.ViewModels
{
    public class LancamentosDoMesViewModel
    {
        [DisplayName("Filtro Conta")]
        [Required(ErrorMessage = "Selecione uma conta")]
        public Guid ContaIdFiltro { get; set; }

        [DisplayName("Transação")]
        public TipoTransacao NovaTransacao { get; set; }

        [DataType(DataType.Currency)]
        public decimal SaldoDoMesAnterior { get; set; }

        [DataType(DataType.Currency)]
        public decimal SaldoDoMesTodasAsContas { get; set; }

        [DataType(DataType.Currency)]
        public decimal SaldoDoMesPorConta { get; set; }

        [DataType(DataType.Currency)]
        public decimal SaldoAtualDoMesPorConta { get; set; }

        public virtual IEnumerable<LancamentoViewModel> LancamentosDoMesPorConta { get; set; }
    }

    public enum TipoTransacao { Receita, Despesa, Transferencia }
}