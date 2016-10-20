using Moneta.Infra.CrossCutting.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Moneta.Application.ViewModels
{
    public class LancamentoViewModel
    {
        public LancamentoViewModel()
        {
            LancamentoId = Guid.NewGuid();
            BaseDaSerie = false;
            DataVencimento = DateTime.Now;
        }

        public LancamentoViewModel(DateTime MesAnoCompetencia)
        {
            LancamentoId = Guid.NewGuid();
            BaseDaSerie = false;

            if (MesAnoCompetencia.Month == DateTime.Now.Month && MesAnoCompetencia.Year == DateTime.Now.Year)
                DataVencimento = DateTime.Now;
            else
                DataVencimento = new DateTime(MesAnoCompetencia.Year, MesAnoCompetencia.Month, 1);
        }

        [Key]
        [DisplayName("Código")]
        public Guid LancamentoId { get; set; }

        [Required(ErrorMessage = "Preencha o campo")]
        [MaxLength(150, ErrorMessage = "Máximo {0} caracteres")]
        [MinLength(2, ErrorMessage = "Mínimo {0} caracteres")]
        [DisplayName("Descrição")]
        public string Descricao { get; set; }

        [DataType(DataType.Currency)]
        [Range(typeof(decimal), "0,01", "9999999,99")]
        [Required(ErrorMessage = "Preencha o campo")]
        [DisplayName("Valor")]
        public decimal Valor { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yy}")]
        [DataType(DataType.DateTime)]
        [DisplayName("Vencimento")]
        public DateTime DataVencimento { get; set; }

        [DisplayName("Pago?")]
        public bool Pago { get; set; }

        [DisplayName("Transação")]
        public TipoTransacaoEnum Transacao { get; set; }

        [DisplayName("Data de Cadastro")]
        [ScaffoldColumn(false)]
        public DateTime DataCadastro { get; set; }

        [ScaffoldColumn(false)]
        public bool Fake { get; set; }

        [ScaffoldColumn(false)]
        public bool BaseDaSerie { get; set; }

        [ScaffoldColumn(false)]
        public string IdDaParcelaNaSerie { get; set; }

        [ScaffoldColumn(false)]
        public bool Ativo { get; set; }

        [DisplayName("Categoria")]
        public Guid CategoriaId { get; set; }

        [DisplayName("Categoria")]
        [JsonIgnore]
        public virtual CategoriaViewModel Categoria { get; set; }

        [DisplayName("Conta")]
        public Guid ContaId { get; set; }

        [DisplayName("Conta")]
        [JsonIgnore]
        public virtual ContaViewModel Conta { get; set; }

        [DisplayName("Lançamento Parcelado")]
        public Guid? LancamentoParceladoId { get; set; }

        [DisplayName("Lançamento Parcelado")]
        [JsonIgnore]
        public virtual LancamentoParceladoViewModel LancamentoParcelado { get; set; }
    }
}