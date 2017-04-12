using Moneta.Infra.CrossCutting.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Moneta.Application.ViewModels
{
    public class ExtratoBancarioViewModel
    {
        [Key]
        [DisplayName("Código")]
        public Guid ExtratoBancarioId { get; set; }

        [Required(ErrorMessage = "Preencha o campo")]
        [MaxLength(150, ErrorMessage = "Máximo {0} caracteres")]
        [MinLength(2, ErrorMessage = "Mínimo {0} caracteres")]
        [DisplayName("Descrição")]
        public string Descricao { get; set; }

        [DataType(DataType.Currency)]
        [Range(typeof(decimal), "-9999999,99", "9999999,99", ErrorMessage="O valor fornecido excede os limites do sistema.")]
        [Required(ErrorMessage = "Preencha o campo")]
        [DisplayName("Valor")]
        public decimal Valor { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yy}")]
        [DataType(DataType.DateTime)]
        [DisplayName("Compensação")]
        public DateTime DataCompensacao { get; set; }

        [DisplayName("N° Documento")]
        public string NumeroDocumento { get; set; }
        
        [DisplayName("Data de Cadastro")]
        [ScaffoldColumn(false)]
        public DateTime DataCadastro { get; set; }

        [DisplayName("Conta")]
        public Guid ContaId { get; set; }

        [DisplayName("Conta")]
        public virtual ContaViewModel Conta { get; set; }

        public bool Selecionado { get; set; }

        public virtual LancamentoViewModel Lancamento { get; set; }
    }
}