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
        public virtual IEnumerable<Lancamento> LancamentosDoMesPorConta { get; set; }
        public decimal SaldoDoMesAnterior { get; set; }
        public decimal SaldoDoMesTodasAsContas { get; set; }
        public decimal SaldoDoMesPorConta { get; set; }
        public decimal SaldoAtualDoMesPorConta { get; set; }
    }
}
