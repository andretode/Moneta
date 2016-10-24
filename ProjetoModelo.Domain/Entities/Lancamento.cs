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
            Ativo = true;
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
        public string IdDaParcelaNaSerie { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool BaseDaSerie { get; set; }
        public bool Ativo { get; set; }

        public bool Fake { get; private set; }
        public ValidationResult ResultadoValidacao { get; private set; }

        public Lancamento CloneFake()
        {
            return CloneFake(DateTime.MinValue);
        }

        public Lancamento CloneFake(DateTime novaDataVencimento)
        {
            var clone = new Lancamento();
            clone.Descricao = this.Descricao;
            clone.Valor = this.Valor;
            clone.Pago = false;
            clone.ContaId = this.ContaId;
            clone.Conta = this.Conta;
            clone.CategoriaId = this.CategoriaId;
            clone.Categoria = this.Categoria;
            clone.LancamentoParceladoId = this.LancamentoParceladoId;
            clone.LancamentoParcelado = this.LancamentoParcelado;
            clone.DataCadastro = this.DataCadastro;
            clone.Ativo = this.Ativo;
            clone.Fake = true;

            if (novaDataVencimento != DateTime.MinValue)
            {
                clone.DataVencimento = (DateTime)novaDataVencimento;
                clone.IdDaParcelaNaSerie = this.LancamentoId.ToString() + novaDataVencimento;
            }

            return clone;
        }

        /// <summary>
        /// Pega a data de vencimento da parcela na série. Ele extrai esta informação apartir do IdDaParcelaNaSerie.
        /// </summary>
        public DateTime GetDataVencimentoDaParcelaNaSerie()
        {
            if (IdDaParcelaNaSerie == null)
                return DateTime.MinValue;

            string strDataVencimentoAnterior = this.IdDaParcelaNaSerie.Substring(36, 10);
            return DateTime.Parse(strDataVencimentoAnterior);
        }

        /// <summary>
        /// Adiciona dias na data de vencimento da parcela na série. Tal informação é atualizada no IdDaParcelaNaSerie.
        /// </summary>
        /// <param name="dias">Quantidades de dias a ser somado na data. Poder ser valor negativo.</param>
        public void AddDaysDataVencimentoDaParcelaNaSerie(double dias)
        {
            if (!this.BaseDaSerie)
                this.IdDaParcelaNaSerie = this.IdDaParcelaNaSerie.Substring(0, 36) + this.GetDataVencimentoDaParcelaNaSerie().AddDays(dias);
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
