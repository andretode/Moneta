using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Moneta.Infra.CrossCutting.Enums;

namespace Moneta.Application.ViewModels
{
    public class LancamentosDoMesViewModel
    {
        [DisplayName("Filtro Conta")]
        [Required(ErrorMessage = "Selecione uma conta")]
        public Guid ContaIdFiltro { get; set; }

        public string PesquisarDescricao { get; set; }
        public DateTime MesAnoCompetencia { get; set; }

        [DisplayName("Transação")]
        public TipoTransacaoEnum NovaTransacao { get; set; }

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
}