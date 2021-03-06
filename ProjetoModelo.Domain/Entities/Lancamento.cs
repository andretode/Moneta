﻿using Moneta.Domain.ValueObjects;
using Moneta.Infra.CrossCutting.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Domain.Entities
{
    public class Lancamento : IExtratoOfx
    {
        public Lancamento()
        {
            Inicializar();
        }

        public Lancamento(IExtratoOfx extratoOfx, Guid grupoLancamentoId)
        {
            Inicializar();
            this.ContaId = extratoOfx.ContaId;
            this.DataCompensacao = extratoOfx.DataCompensacao;
            this.Descricao = extratoOfx.Descricao;
            this.NumeroDocumento = extratoOfx.NumeroDocumento;
            this.Valor = extratoOfx.Valor;
            this.GrupoLancamentoId = grupoLancamentoId;
            this.CategoriaId = new Guid(Categoria.NenhumGuid);
            
            if(Valor > 0)
                this.TipoDeTransacao = TipoTransacaoEnum.Receita;
            else
                this.TipoDeTransacao = TipoTransacaoEnum.Despesa;
        }

        private void Inicializar()
        {
            this.LancamentoId = Guid.NewGuid();
            this.Ativo = true;
            this.Pago = false;
            this.Fake = false;
            this.BaseDaSerie = false;
        }

        public Guid LancamentoId { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime DataCompensacao {
            get { return DataVencimento; }
            set { DataVencimento = value; }
        }
        public bool Pago { get; set; }
        public string Observacao { get; set; }
        public string NumeroDocumento { get; set; }
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
        public TipoTransacaoEnum TipoDeTransacao { get; set; }
        public Guid? LancamentoIdTransferencia { get; set; }
        public virtual Lancamento LancamentoTransferencia { get; set; }
        public Guid? GrupoLancamentoId { get; set; }
        public virtual GrupoLancamento GrupoLancamento { get; set; }
        public Guid? ExtratoBancarioId { get; set; }
        public virtual ExtratoBancario ExtratoBancario { get; set; } 

        public bool Fake { get; private set; }
        public ValidationResult ResultadoValidacao { get; private set; }

        /// <summary>
        /// Retorna a descrição do lançamento e mais o número da parcela quando ela for parcelada.
        /// </summary>
        public string DescricaoMaisNumeroParcela()
        {
            if (this.LancamentoParcelado == null || this.LancamentoParcelado.NumeroParcelas == null || this.LancamentoParcelado.NumeroParcelas < 2)
                return Descricao;

            var dataInicio = this.LancamentoParcelado.DataInicio;
            var dataParcelaNaSerie = GetDataVencimentoDaParcelaNaSerie();
            var numParcela = (dataParcelaNaSerie-dataInicio).Days/this.LancamentoParcelado.Periodicidade + 1;

            return Descricao + " (" + numParcela + "/" + this.LancamentoParcelado.NumeroParcelas + ")";
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
            if (this.IdDaParcelaNaSerie != null && !this.BaseDaSerie)
                this.IdDaParcelaNaSerie = this.IdDaParcelaNaSerie.Substring(0, 36) + this.GetDataVencimentoDaParcelaNaSerie().AddDays(dias);
        }

        public bool IsValid()
        {
            //var fiscal = new LancamentoEstaAptaParaCadastroNoSistema();

            //ResultadoValidacao = fiscal.Validar(this);

            //return ResultadoValidacao.IsValid;
            return true;
        }

        public Lancamento Clone(DateTime novaDataVencimento)
        {
            var clone = new Lancamento();
            clone.Descricao = this.Descricao;
            clone.Valor = this.Valor;
            clone.Pago = clone.Pago;
            clone.ContaId = this.ContaId;
            clone.Conta = this.Conta;
            clone.CategoriaId = this.CategoriaId;
            clone.Categoria = this.Categoria;
            clone.LancamentoParceladoId = this.LancamentoParceladoId;
            clone.LancamentoParcelado = this.LancamentoParcelado;
            clone.DataCadastro = this.DataCadastro;
            clone.Ativo = this.Ativo;
            clone.DataVencimento = novaDataVencimento;
            clone.TipoDeTransacao = this.TipoDeTransacao;
            clone.IdDaParcelaNaSerie = this.LancamentoId.ToString() + novaDataVencimento; //trocar pelo LancamentoParceladoId
            clone.GrupoLancamentoId = this.GrupoLancamentoId;
            clone.GrupoLancamento = this.GrupoLancamento;
            clone.Observacao = this.Observacao;
            clone.Fake = false;

            return clone;
        }

        public Lancamento CloneFake(DateTime novaDataVencimento)
        {
            var clone = this.Clone(novaDataVencimento);
            clone.Fake = true;

            return clone;
        }

        public Lancamento CreateLancamentoTransferenciaPar(Guid ContaIdDestino)
        {
            var lancamentoDestino = new Lancamento();
            this.LancamentoIdTransferencia = lancamentoDestino.LancamentoId;
            lancamentoDestino.LancamentoIdTransferencia = this.LancamentoId;
            lancamentoDestino.ContaId = ContaIdDestino;
            lancamentoDestino.Descricao = this.Descricao;
            lancamentoDestino.Valor = this.Valor * -1;
            lancamentoDestino.DataVencimento = this.DataVencimento;
            lancamentoDestino.CategoriaId = this.CategoriaId;
            lancamentoDestino.TipoDeTransacao = this.TipoDeTransacao;
            lancamentoDestino.Ativo = this.Ativo;
            lancamentoDestino.TipoDeTransacao = this.TipoDeTransacao;

            return lancamentoDestino;
        }

        public bool WasImported(IExtratoOfx extrato)
        {
            return extrato.Descricao.Equals(this.Descricao) &&
                extrato.DataCompensacao.Equals(this.DataCompensacao) &&
                extrato.Valor.Equals(this.Valor);
        }
    }
}
