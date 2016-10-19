using Moneta.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Domain.Entities
{
    public class Lancamento
    {
        public Lancamento()
        {
            LancamentoId = Guid.NewGuid();
            Pago = false;
            Fake = false;
            BaseDaSerie = false;
        }

        public Guid LancamentoId { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public bool Pago { get; set; }
        public Guid ContaId { get; set; }
        public virtual Conta Conta { get; set; }
        public Guid CategoriaId { get; set; }
        public virtual Categoria Categoria { get; set; }
        public Guid? LancamentoParceladoId { get; set; }
        public virtual LancamentoParcelado LancamentoParcelado { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool BaseDaSerie { get; set; }

        public bool Fake { get; private set; }
        public ValidationResult ResultadoValidacao { get; private set; }

        public Lancamento CloneFake()
        {
            var clone = new Lancamento();
            clone.Descricao = this.Descricao;
            clone.Valor = this.Valor;
            clone.DataVencimento = this.DataVencimento;
            clone.Pago = false;
            clone.ContaId = this.ContaId;
            clone.Conta = this.Conta;
            clone.CategoriaId = this.CategoriaId;
            clone.Categoria = this.Categoria;
            clone.LancamentoParceladoId = this.LancamentoParceladoId;
            clone.LancamentoParcelado = this.LancamentoParcelado;
            clone.DataCadastro = this.DataCadastro;
            clone.Fake = true;

            return clone;
        }

        public bool IsValid()
        {
            //var fiscal = new LancamentoEstaAptaParaCadastroNoSistema();

            //ResultadoValidacao = fiscal.Validar(this);

            //return ResultadoValidacao.IsValid;
            return true;
        }
    }
}
