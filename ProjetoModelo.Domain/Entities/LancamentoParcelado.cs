using Moneta.Domain.ValueObjects;
using Moneta.Infra.CrossCutting.Enums;
using System;
using System.Collections.Generic;

namespace Moneta.Domain.Entities
{
    public class LancamentoParcelado
    {
        public LancamentoParcelado()
        {
            LancamentoParceladoId = Guid.NewGuid();
        }
        public Guid LancamentoParceladoId { get; set; }
        public int? NumeroParcelas { get; set; }
        public int Periodicidade { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataCadastro { get; set; }
        public Guid LancamentoBaseId { get; set; }
        public TipoDeAlteracaoDaRepeticaoEnum TipoDeAlteracaoDaRepeticao { get; set; }
        //public virtual Lancamento LacamentoBase { get; set; }
        public virtual ICollection<Lancamento> Lancamentos { get; set; }
        public ValidationResult ResultadoValidacao { get; private set; }

        public TipoRepeticao TipoDeRepeticao()
        {
            var tipoRepeticao = TipoRepeticao.Parcelado;
            if (this.NumeroParcelas == null)
                tipoRepeticao = TipoRepeticao.Fixo;

            return tipoRepeticao;
        }

        public bool IsValid()
        {
            return true;
        }
    }
}
