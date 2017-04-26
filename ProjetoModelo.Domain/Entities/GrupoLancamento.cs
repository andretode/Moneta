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
        public Guid ContaId { get; set; }
        public virtual Conta Conta { get; set; }
        public Guid? ExtratoBancarioId { get; set; }
        public virtual ExtratoBancario ExtratoBancario { get; set; } 
        public DateTime DataCadastro { get; set; }
        public ValidationResult ResultadoValidacao { get; private set; }

        public bool IsValid()
        {
            return true;
        }

        public decimal Valor
        {
            get
            {
                return Lancamentos.Select(l => l.Valor).Sum();
            }
        }

        public bool Pago
        {
            get
            {
                if (Lancamentos != null)
                    return Lancamentos.Where(l => l.Pago).Count() > 0;
                else
                    return false;
            }
            set
            {
                if (Lancamentos != null)
                {
                    foreach (var lancamento in Lancamentos)
                        lancamento.Pago = value;
                }
            }            
        }

    }
}
