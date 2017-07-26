using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Moneta.Application.ViewModels
{
    public class CategoriaViewModel
    {
        public CategoriaViewModel()
        {
            CategoriaId = Guid.NewGuid();
        }

        [Key]
        [DisplayName("Código")]
        public Guid CategoriaId { get; set; }

        [Required(ErrorMessage = "Preencha o campo")]
        [MaxLength(150, ErrorMessage = "Máximo {0} caracteres")]
        [MinLength(2, ErrorMessage = "Mínimo {0} caracteres")]
        [DisplayName("Descrição")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Preencha o campo")]
        [MaxLength(7, ErrorMessage = "Máximo {0} caracteres")]
        [MinLength(7, ErrorMessage = "Mínimo {0} caracteres")]
        [DisplayName("Cor")]
        public string Cor { get; set; }

        [UIHint("DinheiroColorido")]
        [Range(typeof(decimal), "-9999999,99", "9999999,99", ErrorMessage = "O valor fornecido excede os limites do sistema.")]
        [Required(ErrorMessage = "Preencha o campo")]
        [DisplayName("Orçamento Mensal")]
        [JsonIgnore]
        public decimal OrcamentoMensal { get; set; }

        [DisplayName("Data de Cadastro")]
        [ScaffoldColumn(false)]
        [JsonIgnore]
        public DateTime DataCadastro { get; set; }

        public const string Nenhum = "Nenhum";

        [JsonIgnore]
        public ICollection<LancamentoViewModel> Lancamentos { get; set; }

    }
}