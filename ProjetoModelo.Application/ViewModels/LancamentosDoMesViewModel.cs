using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Moneta.Infra.CrossCutting.Enums;

namespace Moneta.Application.ViewModels
{
    public class LancamentosDoMesViewModel
    {
        public Guid? contaIdFiltro = Guid.Empty;

        [DisplayName("Filtro Conta")]
        public Guid? ContaIdFiltro {
            get { return contaIdFiltro; }
            set { contaIdFiltro = value; }
        }

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