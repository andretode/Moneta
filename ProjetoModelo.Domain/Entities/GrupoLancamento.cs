using Moneta.Domain.ValueObjects;
using System;
using System.Linq;
using System.Collections.Generic;

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
        public virtual ICollection<Lancamento> Lancamentos { get; set; }
        public DateTime DataCadastro { get; set; }
        public ValidationResult ResultadoValidacao { get; private set; }

        public bool IsValid()
        {
            return true;
        }

        public bool Pago
        {
            get
            {
                return Lancamentos.Where(l => l.Pago).Count() > 0;
            }
            set
            {
                foreach (var lancamento in Lancamentos)
                    lancamento.Pago = value;
            }            
        }

    }
}
