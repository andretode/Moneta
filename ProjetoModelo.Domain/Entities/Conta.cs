using Moneta.Domain.Validation.Contas;
using Moneta.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Domain.Entities
{
    public class Conta
    {
        public Conta()
        {
            ContaId = Guid.NewGuid();
        }

        public Guid ContaId { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public virtual ICollection<Lancamento> Lancamentos { get; set; }
        public virtual ICollection<ExtratoBancario> ExtratosBancarios { get; set; }
        public DateTime DataCadastro { get; set; }

        public ValidationResult ResultadoValidacao { get; private set; }

        public bool IsValid()
        {
            var fiscal = new ContaEstaAptoParaCadastroNoSistema();

            ResultadoValidacao = fiscal.Validar(this);

            return ResultadoValidacao.IsValid;
        }
    }
}
