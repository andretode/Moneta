using Moneta.Infra.CrossCutting.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Moneta.Application.ViewModels
{
    public class LancamentoParceladoViewModel
    {
        public LancamentoParceladoViewModel()
        {
            LancamentoParceladoId = Guid.NewGuid();
        }

        [Key]
        [DisplayName("Código")]
        public Guid LancamentoParceladoId { get; set; }

        [DisplayName("N° de Parcelas")]
        public int? NumeroParcelas { get; set; }

        [DisplayName("Periodicidade")]
        public PeriodicidadeEnum Periodicidade { get; set; }

        [DisplayName("Repetição")]
        public TipoRepeticao TipoDeRepeticao { get; set; }

        public TipoDeAlteracaoDaRepeticaoEnum TipoDeAlteracaoDaRepeticao { get; set; }

        [DisplayName("Data Início das Parcelas")]
        public DateTime DataInicio { get; set; }

        [ScaffoldColumn(false)]
        public Guid LancamentoBaseId { get; set; }

        [JsonIgnore]
        public virtual ICollection<LancamentoViewModel> Lancamentos { get; set; }

        [DisplayName("Data de Cadastro")]
        [ScaffoldColumn(false)]
        public DateTime DataCadastro { get; set; }
    }
}