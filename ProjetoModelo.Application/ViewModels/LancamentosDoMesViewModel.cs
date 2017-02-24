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

        public Guid? GrupoLancamentoId { get; set; }

        public string PesquisarDescricao { get; set; }
        public DateTime MesAnoCompetencia { get; set; }

        [DisplayName("Transação")]
        public TipoTransacaoEnum NovaTransacao { get; set; }

        [DataType(DataType.Currency)]
        public decimal SaldoDoMesAnterior { get; set; }

        [DataType(DataType.Currency)]
        public decimal SaldoDoMes { get; set; }

        [DataType(DataType.Currency)]
        public decimal SaldoAtualDoMes { get; set; }

        [DataType(DataType.Currency)]
        public decimal ReceitaTotal { get; set; }

        [DataType(DataType.Currency)]
        public decimal DespesaTotal { get; set; }

        public virtual IEnumerable<LancamentoViewModel> LancamentosDoMes { get; set; }
        public virtual IEnumerable<LancamentoAgrupadoViewModel> LancamentosAgrupados { get; set; }
    }
}