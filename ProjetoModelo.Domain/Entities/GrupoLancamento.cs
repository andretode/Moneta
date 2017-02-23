using Moneta.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Domain.Entities
{
    public class GrupoLancamento
    {
        public GrupoLancamento()
        {
            GrupoLancamentoId = Guid.NewGuid();
        }

        public Guid GrupoLancamentoId { get; set; }
        public string Descricao { get; set; }
        public DateTime DataVencimento { get; set; }
        public virtual IEnumerable<Lancamento> Lancamentos { get; set; }
        public DateTime DataCadastro { get; set; }
        public ValidationResult ResultadoValidacao { get; private set; }

        public bool IsValid()
        {
            return true;
        }
    }
}
