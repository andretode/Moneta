using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Moneta.Application.ViewModels
{
    public class GrupoLancamentoViewModel
    {
        public GrupoLancamentoViewModel()
        {
            GrupoLancamentoId = Guid.NewGuid();
        }

        [Key]
        [DisplayName("Código")]
        public Guid GrupoLancamentoId { get; set; }

        [Required(ErrorMessage = "Preencha o campo")]
        [MaxLength(150, ErrorMessage = "Máximo {0} caracteres")]
        [MinLength(2, ErrorMessage = "Mínimo {0} caracteres")]
        [DisplayName("Descrição")]
        public string Descricao { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yy}")]
        [DataType(DataType.DateTime)]
        [DisplayName("Vencimento")]
        public DateTime DataVencimento { get; set; }

        [DisplayName("Pago?")]
        public bool Pago { get; set; }

        [DisplayName("Conta")]
        public Guid ContaId { get; set; }

        [DisplayName("Conta")]
        [JsonIgnore]
        public virtual ContaViewModel Conta { get; set; }

        [DisplayName("Data de Cadastro")]
        [ScaffoldColumn(false)]
        public DateTime DataCadastro { get; set; }
        
        public ICollection<LancamentoViewModel> Lancamentos { get; set; }

    }
}