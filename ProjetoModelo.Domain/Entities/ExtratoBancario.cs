﻿using Moneta.Domain.ValueObjects;
using System;

namespace Moneta.Domain.Entities
{
    public class ExtratoBancario : IExtratoOfx
    {
        public ExtratoBancario()
        {
            ExtratoBancarioId = Guid.NewGuid();
        }

        public Guid ExtratoBancarioId { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataCompensacao { get; set; }
        public string NumeroDocumento { get; set; }
        public Guid ContaId { get; set; }
        public virtual Conta Conta { get; set; }
        public DateTime DataCadastro { get; set; }

        public ValidationResult ResultadoValidacao { get; private set; }

        public virtual Lancamento Lancamento { get; set; }
        public virtual GrupoLancamento GrupoLancamento { get; set; }

        public bool IsValid()
        {
            return true;
        }

        public bool WasImported(IExtratoOfx extrato)
        {
            return extrato.Descricao.Equals(this.Descricao) &&
                extrato.DataCompensacao.Equals(this.DataCompensacao) &&
                extrato.Valor.Equals(this.Valor);
        }
    }
}
