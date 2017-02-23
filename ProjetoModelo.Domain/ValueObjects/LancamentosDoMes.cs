using Moneta.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Domain.ValueObjects
{
    public class AgregadoLancamentosDoMes
    {
        public Guid ContaIdFiltro { get; set; }
        public string PesquisarDescricao { get; set; }
        public DateTime MesAnoCompetencia { get; set; }
        public virtual IEnumerable<Lancamento> LancamentosDoMes { get; set; }
        public virtual IEnumerable<LancamentoAgrupado> LancamentosAgrupados { get; set; }
        public decimal SaldoDoMesAnterior { get; set; }
        public decimal SaldoDoMes { get; set; }
        public decimal SaldoAtualDoMes { get; set; }
        public decimal ReceitaTotal { get; set; }
        public decimal DespesaTotal { get; set; }
    }
}
